<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import type { AnalyticsResponse, ChatRequest, ChatResponse, Message } from "./types";

// Environment variables
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
const apiKey = import.meta.env.VITE_API_KEY;
// Analytics state
const viewCount = ref<number>(0);
const lastUpdated = ref<string>("");
const analyticsLoading = ref<boolean>(false);
const analyticsError = ref<string>("");
const pageId = ref<string>("home");

// Chat state
const messages = ref<Message[]>([]);
const userInput = ref<string>("");
const chatLoading = ref<boolean>(false);
const chatError = ref<string>("");

// UI state
const activeTab = ref<'dashboard' | 'chat' | 'both'>("dashboard");
const animateCounter = ref<boolean>(false);

// Fetch analytics
async function fetchAnalytics(): Promise<void> {
  analyticsLoading.value = true;
  analyticsError.value = "";
  const previousCount = viewCount.value;
  
  try {
    const res = await fetch(
      `${apiBaseUrl}/api/pageview/visitor-count?pageId=${pageId.value}&code=${apiKey}`
    );
    
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }
    
    const data = (await res.json()) as AnalyticsResponse;
    viewCount.value = data.ViewCount ?? 0;
    lastUpdated.value = data.LastUpdated ?? "";
    
    // Trigger animation if count changed
    if (previousCount !== viewCount.value && previousCount > 0) {
      animateCounter.value = true;
      setTimeout(() => (animateCounter.value = false), 600);
    }
  } catch (e: any) {
    analyticsError.value = e.message ?? "Unknown error";
  } finally {
    analyticsLoading.value = false;
  }
}

// Send chat message
async function sendMessage(): Promise<void> {
  if (!userInput.value.trim()) return;
  
  const userMsg = userInput.value.trim();
  messages.value.push({ role: "user", content: userMsg });
  userInput.value = "";
  chatLoading.value = true;
  chatError.value = "";
  
  try {
    const request: ChatRequest = { Message: userMsg };
    const res = await fetch(`${apiBaseUrl}/api/llm-chat/chat?code=${apiKey}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });
    
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }
    
    const data = (await res.json()) as ChatResponse;
    messages.value.push({ 
      role: "assistant", 
      content: data.Response ?? "No response from AI" 
    });
  } catch (e: any) {
    chatError.value = e.message ?? "Unknown error";
    messages.value.push({ 
      role: "error", 
      content: `Error: ${e.message}` 
    });
  } finally {
    chatLoading.value = false;
  }
}

// Quick action: refresh + ask AI about stats
async function askAboutStats(): Promise<void> {
  await fetchAnalytics();
  userInput.value = `I currently have ${viewCount.value} page views. Can you give me encouraging feedback and tips to increase engagement?`;
  await sendMessage();
}

// Format timestamp
const formattedTime = computed(() => {
  if (!lastUpdated.value) return "Never";
  try {
    return new Date(lastUpdated.value).toLocaleString("en-US", {
      dateStyle: "medium",
      timeStyle: "short",
    });
  } catch {
    return lastUpdated.value;
  }
});

onMounted(() => {
  fetchAnalytics();
  // Add welcome message
  messages.value.push({
    role: "assistant",
    content: "👋 Hi! I'm your AI portfolio assistant. Ask me anything about web development, career advice, or your analytics!",
  });
});
</script>

<template>
  <div class="app-container">
    <!-- Header -->
    <header class="header">
      <h1>📊 Portfolio Hub</h1>
      <div class="tab-switcher">
        <button 
          @click="activeTab = 'dashboard'" 
          :class="{ active: activeTab === 'dashboard' }"
        >
          Analytics
        </button>
        <button 
          @click="activeTab = 'chat'" 
          :class="{ active: activeTab === 'chat' }"
        >
          AI Assistant
        </button>
        <button 
          @click="activeTab = 'both'" 
          :class="{ active: activeTab === 'both' }"
        >
          Split View
        </button>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main-content" :class="`layout-${activeTab}`">
      <!-- Analytics Panel -->
      <section v-show="activeTab === 'dashboard' || activeTab === 'both'" class="panel analytics-panel">
        <div class="panel-header">
          <h2>📈 Live Analytics</h2>
          <button @click="fetchAnalytics" :disabled="analyticsLoading" class="btn-refresh">
            {{ analyticsLoading ? "⏳" : "🔄" }} Refresh
          </button>
        </div>

        <div v-if="analyticsError" class="error-box">
          ⚠️ {{ analyticsError }}
        </div>

        <div v-else class="stats-grid">
          <div class="stat-card" :class="{ pulse: animateCounter }">
            <div class="stat-icon">👁️</div>
            <div class="stat-value">{{ viewCount.toLocaleString() }}</div>
            <div class="stat-label">Total Views</div>
          </div>

          <div class="stat-card">
            <div class="stat-icon">🕒</div>
            <div class="stat-value-small">{{ formattedTime }}</div>
            <div class="stat-label">Last Updated</div>
          </div>

          <div class="stat-card">
            <div class="stat-icon">📄</div>
            <div class="stat-value-small">{{ pageId }}</div>
            <div class="stat-label">Page ID</div>
          </div>
        </div>

        <div class="quick-actions">
          <h3>Quick Actions</h3>
          <button @click="askAboutStats" class="btn-action">
            🤖 Ask AI About My Stats
          </button>
          <button @click="pageId = 'home'; fetchAnalytics()" class="btn-action">
            🏠 View Home Stats
          </button>
          <button @click="pageId = 'portfolio'; fetchAnalytics()" class="btn-action">
            💼 View Portfolio Stats
          </button>
        </div>
      </section>

      <!-- Chat Panel -->
      <section v-show="activeTab === 'chat' || activeTab === 'both'" class="panel chat-panel">
        <div class="panel-header">
          <h2>💬 AI Assistant</h2>
          <button @click="messages = []" class="btn-clear">Clear Chat</button>
        </div>

        <div class="chat-messages">
          <div 
            v-for="(msg, idx) in messages" 
            :key="idx" 
            :class="['message', `message-${msg.role}`]"
          >
            <div class="message-icon">
              {{ msg.role === 'user' ? '👤' : msg.role === 'error' ? '❌' : '🤖' }}
            </div>
            <div class="message-content">{{ msg.content }}</div>
          </div>

          <div v-if="chatLoading" class="message message-assistant">
            <div class="message-icon">🤖</div>
            <div class="message-content typing">AI is thinking...</div>
          </div>
        </div>

        <form @submit.prevent="sendMessage" class="chat-input-form">
          <input 
            v-model="userInput" 
            placeholder="Ask me anything..." 
            :disabled="chatLoading"
            class="chat-input"
          />
          <button type="submit" :disabled="chatLoading || !userInput.trim()" class="btn-send">
            {{ chatLoading ? "⏳" : "➤" }}
          </button>
        </form>

        <div v-if="chatError" class="error-box">⚠️ {{ chatError }}</div>
      </section>
    </main>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.app-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #333;
}

.header {
  background: rgba(255, 255, 255, 0.95);
  padding: 1.5rem 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
}

.header h1 {
  margin: 0;
  font-size: 1.8rem;
  background: linear-gradient(135deg, #667eea, #764ba2);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.tab-switcher {
  display: flex;
  gap: 0.5rem;
}

.tab-switcher button {
  padding: 0.6rem 1.2rem;
  border: 2px solid #667eea;
  background: white;
  color: #667eea;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.tab-switcher button.active {
  background: #667eea;
  color: white;
}

.tab-switcher button:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(102, 126, 234, 0.3);
}

.main-content {
  padding: 2rem;
  display: grid;
  gap: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.main-content.layout-dashboard,
.main-content.layout-chat {
  grid-template-columns: 1fr;
}

.main-content.layout-both {
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
}

.panel {
  background: white;
  border-radius: 16px;
  padding: 2rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 2px solid #f0f0f0;
  padding-bottom: 1rem;
}

.panel-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.btn-refresh,
.btn-clear {
  padding: 0.5rem 1rem;
  border: none;
  background: #667eea;
  color: white;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.btn-refresh:hover,
.btn-clear:hover {
  background: #5568d3;
  transform: scale(1.05);
}

.btn-refresh:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
  transition: transform 0.3s;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-card.pulse {
  animation: pulse 0.6s ease-in-out;
}

@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

.stat-icon {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #667eea;
}

.stat-value-small {
  font-size: 1.2rem;
  font-weight: 600;
  color: #555;
}

.stat-label {
  font-size: 0.9rem;
  color: #666;
  margin-top: 0.5rem;
}

.quick-actions {
  border-top: 2px solid #f0f0f0;
  padding-top: 1rem;
}

.quick-actions h3 {
  margin: 0 0 1rem 0;
  font-size: 1.1rem;
}

.btn-action {
  display: block;
  width: 100%;
  padding: 0.8rem;
  margin-bottom: 0.5rem;
  border: 2px solid #764ba2;
  background: white;
  color: #764ba2;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.btn-action:hover {
  background: #764ba2;
  color: white;
}

.chat-panel {
  max-height: 80vh;
  display: flex;
  flex-direction: column;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding-right: 0.5rem;
}

.message {
  display: flex;
  gap: 0.8rem;
  align-items: flex-start;
}

.message-user {
  flex-direction: row-reverse;
}

.message-icon {
  font-size: 1.8rem;
  flex-shrink: 0;
}

.message-content {
  background: #f5f5f5;
  padding: 0.8rem 1.2rem;
  border-radius: 12px;
  max-width: 70%;
  line-height: 1.5;
}

.message-user .message-content {
  background: #667eea;
  color: white;
}

.message-error .message-content {
  background: #ffebee;
  color: #c62828;
}

.typing {
  animation: typing 1.5s infinite;
}

@keyframes typing {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.chat-input-form {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.chat-input {
  flex: 1;
  padding: 0.8rem 1.2rem;
  border: 2px solid #ddd;
  border-radius: 24px;
  font-size: 1rem;
  transition: border 0.3s;
}

.chat-input:focus {
  outline: none;
  border-color: #667eea;
}

.btn-send {
  padding: 0.8rem 1.5rem;
  border: none;
  background: #667eea;
  color: white;
  border-radius: 24px;
  cursor: pointer;
  font-size: 1.2rem;
  transition: all 0.3s;
}

.btn-send:hover:not(:disabled) {
  background: #5568d3;
  transform: scale(1.05);
}

.btn-send:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-box {
  background: #ffebee;
  color: #c62828;
  padding: 1rem;
  border-radius: 8px;
  border-left: 4px solid #c62828;
}

@media (max-width: 768px) {
  .main-content.layout-both {
    grid-template-columns: 1fr;
  }
  
  .header {
    flex-direction: column;
    align-items: stretch;
  }
  
  .tab-switcher {
    justify-content: center;
  }
}
</style>