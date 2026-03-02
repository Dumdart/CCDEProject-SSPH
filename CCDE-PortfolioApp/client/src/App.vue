<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import type { AnalyticsResponse, ChatRequest, ChatResponse, Message, UploadedFile } from "./types";
import ChatMessages from "./components/ChatMessages.vue";

// Configuration
const config = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL,
  apiKey: import.meta.env.VITE_API_KEY,
};

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

// Document analysis state
const uploadedFiles = ref<UploadedFile[]>([]);
const analyzingDoc = ref<boolean>(false);
const analysisResult = ref<string>("");

// UI state
const activeTab = ref<"dashboard" | "chat" | "documents" | "split">("dashboard");
const animateCounter = ref<boolean>(false);

// Fetch analytics
async function fetchAnalytics(): Promise<void> {
  analyticsLoading.value = true;
  analyticsError.value = "";
  const previousCount = viewCount.value;

  try {
    const res = await fetch(
      `${config.apiBaseUrl}/api/pageview/visitor-count?pageId=${pageId.value}&code=${config.apiKey}`
    );

    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }

    const data = (await res.json()) as AnalyticsResponse;
    viewCount.value = data.ViewCount ?? 0;
    lastUpdated.value = data.LastUpdated ?? "";

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
    const res = await fetch(`${config.apiBaseUrl}/api/llm-chat/chat?code=${config.apiKey}`, {
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

// Handle file upload
function handleFileUpload(event: Event): void {
  const target = event.target as HTMLInputElement;
  const files = target.files;

  if (!files || files.length === 0) return;

  Array.from(files).forEach(file => {
    if (file.size > 10 * 1024 * 1024) {
      alert(`File ${file.name} is too large (max 10MB)`);
      return;
    }

    const reader = new FileReader();
    reader.onload = (e) => {
      uploadedFiles.value.push({
        name: file.name,
        size: file.size,
        type: file.type,
        content: e.target?.result || null
      });
    };
    reader.readAsText(file);
  });

  target.value = "";
}

// Remove uploaded file
function removeFile(index: number): void {
  uploadedFiles.value.splice(index, 1);
}

// Analyze document
async function analyzeDocument(file: UploadedFile): Promise<void> {
  analyzingDoc.value = true;
  analysisResult.value = "";

  try {
    const prompt = `Please analyze this document:

Filename: ${file.name}
Content:
${file.content}`;
    const request: ChatRequest = { Message: prompt };

    const res = await fetch(`${config.apiBaseUrl}/api/llm-chat/chat?code=${config.apiKey}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });

    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }

    const data = (await res.json()) as ChatResponse;
    analysisResult.value = data.Response ?? "No analysis available";
  } catch (e: any) {
    analysisResult.value = `Error: ${e.message}`;
  } finally {
    analyzingDoc.value = false;
  }
}

// Quick action: ask about stats
async function askAboutStats(): Promise<void> {
  await fetchAnalytics();
  userInput.value = `I currently have ${viewCount.value} page views. Can you give me insights and tips to increase engagement?`;
  await sendMessage();
}

// Format file size
function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + " B";
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
  return (bytes / (1024 * 1024)).toFixed(1) + " MB";
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
  messages.value.push({
    role: "assistant",
    content: "Hi! I'm your AI assistant. Ask me anything about web development, career advice, or your analytics!",
  });
});
</script>

<template>
  <div class="app-container">
    <!-- Header -->
    <header class="header">
      <h1>Portfolio Hub</h1>
      <div class="tab-switcher">
        <button @click="activeTab = 'dashboard'" :class="{ active: activeTab === 'dashboard' }">
          Analytics
        </button>
        <button @click="activeTab = 'chat'" :class="{ active: activeTab === 'chat' }">
          AI Assistant
        </button>
        <button @click="activeTab = 'documents'" :class="{ active: activeTab === 'documents' }">
          Documents
        </button>
        <button @click="activeTab = 'split'" :class="{ active: activeTab === 'split' }">
          Split View
        </button>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main-content" :class="`layout-${activeTab}`">
      <!-- Analytics Panel -->
      <section v-show="activeTab === 'dashboard' || activeTab === 'split'" class="panel analytics-panel">
        <div class="panel-header">
          <h2>Live Analytics</h2>
          <button @click="fetchAnalytics" :disabled="analyticsLoading" class="btn-refresh">
            {{ analyticsLoading ? "Loading..." : "Refresh" }}
          </button>
        </div>

        <div v-if="analyticsError" class="error-box">
          Error: {{ analyticsError }}
        </div>

        <div v-else class="stats-grid">
          <div class="stat-card" :class="{ pulse: animateCounter }">
            <div class="stat-value">{{ viewCount.toLocaleString() }}</div>
            <div class="stat-label">Total Views</div>
          </div>

          <div class="stat-card">
            <div class="stat-value-small">{{ formattedTime }}</div>
            <div class="stat-label">Last Updated</div>
          </div>

          <div class="stat-card">
            <div class="stat-value-small">{{ pageId }}</div>
            <div class="stat-label">Current Page</div>
          </div>
        </div>

        <div class="quick-actions">
          <h3>Quick Actions</h3>
          <button @click="askAboutStats" class="btn-action">
            Ask AI About Stats
          </button>
          <button @click="pageId = 'home'; fetchAnalytics()" class="btn-action">
            View Home Stats
          </button>
          <button @click="pageId = 'portfolio'; fetchAnalytics()" class="btn-action">
            View Portfolio Stats
          </button>
          <button @click="pageId = 'projects'; fetchAnalytics()" class="btn-action">
            View Projects Stats
          </button>
        </div>
      </section>

      <!-- Chat Panel -->
      <section v-show="activeTab === 'chat' || activeTab === 'split'" class="panel chat-panel">
        <div class="panel-header">
          <h2>AI Assistant</h2>
          <button @click="messages = []" class="btn-clear">Clear Chat</button>
        </div>

        <ChatMessages :messages="messages" :is-thinking="chatLoading" />

        <form @submit.prevent="sendMessage" class="chat-input-form">
          <input v-model="userInput" placeholder="Ask me anything..." :disabled="chatLoading" class="chat-input" />
          <button type="submit" :disabled="chatLoading || !userInput.trim()" class="btn-send">
            {{ chatLoading ? "..." : "Send" }}
          </button>
        </form>

        <div v-if="chatError" class="error-box">Error: {{ chatError }}</div>
      </section>


      <!-- Documents Panel -->
      <section v-show="activeTab === 'documents'" class="panel documents-panel">
        <div class="panel-header">
          <h2>Document Analysis</h2>
          <label class="btn-upload">
            Upload Files
            <input type="file" @change="handleFileUpload" accept=".pdf,.txt,.docx,.md" multiple
              style="display: none;" />
          </label>
        </div>

        <div class="upload-info">
          Supported formats: PDF, TXT, DOCX, MD (max 10MB per file)
        </div>

        <div v-if="uploadedFiles.length === 0" class="empty-state">
          No documents uploaded yet. Upload files to analyze them with AI.
        </div>

        <div v-else class="files-list">
          <div v-for="(file, idx) in uploadedFiles" :key="idx" class="file-item">
            <div class="file-info">
              <div class="file-name">{{ file.name }}</div>
              <div class="file-meta">{{ formatFileSize(file.size) }}</div>
            </div>
            <div class="file-actions">
              <button @click="analyzeDocument(file)" :disabled="analyzingDoc" class="btn-analyze">
                {{ analyzingDoc ? "Analyzing..." : "Analyze" }}
              </button>
              <button @click="removeFile(idx)" class="btn-remove">Remove</button>
            </div>
          </div>
        </div>

        <div v-if="analysisResult" class="analysis-result">
          <h3>Analysis Result</h3>
          <div class="result-content">{{ analysisResult }}</div>
        </div>
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
  background: #f5f7fa;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  color: #2c3e50;
}

.header {
  background: #ffffff;
  padding: 1.5rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
  border-bottom: 3px solid #3498db;
}

.header h1 {
  margin: 0;
  font-size: 1.8rem;
  color: #2c3e50;
  font-weight: 600;
}

.tab-switcher {
  display: flex;
  gap: 0.5rem;
}

.tab-switcher button {
  padding: 0.6rem 1.2rem;
  border: 2px solid #3498db;
  background: #ffffff;
  color: #3498db;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.tab-switcher button.active {
  background: #3498db;
  color: #ffffff;
}

.tab-switcher button:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(52, 152, 219, 0.3);
}

.main-content {
  padding: 2rem;
  display: grid;
  gap: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.main-content.layout-dashboard,
.main-content.layout-chat,
.main-content.layout-documents {
  grid-template-columns: 1fr;
}

.main-content.layout-split {
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
}

.panel {
  background: #ffffff;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  border: 1px solid #e1e8ed;
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
  font-size: 1.4rem;
  color: #2c3e50;
  font-weight: 600;
}

.btn-refresh,
.btn-clear,
.btn-upload {
  padding: 0.6rem 1.2rem;
  border: none;
  background: #3498db;
  color: #ffffff;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.btn-refresh:hover,
.btn-clear:hover,
.btn-upload:hover {
  background: #2980b9;
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
  background: #f8f9fa;
  border: 2px solid #e1e8ed;
  border-radius: 8px;
  padding: 1.5rem;
  text-align: center;
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  border-color: #3498db;
}

.stat-card.pulse {
  animation: pulse 0.6s ease-in-out;
}

@keyframes pulse {

  0%,
  100% {
    transform: scale(1);
  }

  50% {
    transform: scale(1.05);
  }
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #3498db;
  margin-bottom: 0.5rem;
}

.stat-value-small {
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.9rem;
  color: #7f8c8d;
  font-weight: 500;
}

.quick-actions {
  border-top: 2px solid #f0f0f0;
  padding-top: 1rem;
}

.quick-actions h3 {
  margin: 0 0 1rem 0;
  font-size: 1.1rem;
  color: #2c3e50;
}

.btn-action {
  display: block;
  width: 100%;
  padding: 0.8rem;
  margin-bottom: 0.5rem;
  border: 2px solid #3498db;
  background: #ffffff;
  color: #3498db;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.btn-action:hover {
  background: #3498db;
  color: #ffffff;
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
  min-height: 400px;
}

.message {
  display: flex;
  align-items: flex-start;
}

.message-user {
  justify-content: flex-end;
}

.message-content {
  background: #f8f9fa;
  padding: 0.9rem 1.2rem;
  border-radius: 8px;
  max-width: 75%;
  line-height: 1.6;
  border: 1px solid #e1e8ed;
}

.message-user .message-content {
  background: #3498db;
  color: #ffffff;
  border-color: #3498db;
}

.message-error .message-content {
  background: #fee;
  color: #c00;
  border-color: #fcc;
}

.typing {
  animation: typing 1.5s infinite;
}

@keyframes typing {

  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.5;
  }
}

.chat-input-form {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.chat-input {
  flex: 1;
  padding: 0.9rem 1.2rem;
  border: 2px solid #e1e8ed;
  border-radius: 6px;
  font-size: 1rem;
  transition: border 0.2s;
  font-family: inherit;
}

.chat-input:focus {
  outline: none;
  border-color: #3498db;
}

.btn-send {
  padding: 0.9rem 1.8rem;
  border: none;
  background: #3498db;
  color: #ffffff;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 0.9rem;
}

.btn-send:hover:not(:disabled) {
  background: #2980b9;
}

.btn-send:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.upload-info {
  background: #f8f9fa;
  padding: 0.8rem;
  border-radius: 6px;
  font-size: 0.85rem;
  color: #7f8c8d;
  border: 1px solid #e1e8ed;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #7f8c8d;
  font-size: 1rem;
}

.files-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.file-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #f8f9fa;
  border: 2px solid #e1e8ed;
  border-radius: 6px;
  transition: border-color 0.2s;
}

.file-item:hover {
  border-color: #3498db;
}

.file-info {
  flex: 1;
}

.file-name {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.3rem;
}

.file-meta {
  font-size: 0.85rem;
  color: #7f8c8d;
}

.file-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-analyze,
.btn-remove {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 0.85rem;
}

.btn-analyze {
  background: #3498db;
  color: #ffffff;
}

.btn-analyze:hover:not(:disabled) {
  background: #2980b9;
}

.btn-analyze:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-remove {
  background: #e74c3c;
  color: #ffffff;
}

.btn-remove:hover {
  background: #c0392b;
}

.analysis-result {
  margin-top: 1rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border: 2px solid #3498db;
  border-radius: 6px;
}

.analysis-result h3 {
  margin: 0 0 1rem 0;
  color: #2c3e50;
  font-size: 1.1rem;
}

.result-content {
  line-height: 1.6;
  color: #2c3e50;
  white-space: pre-wrap;
}

.error-box {
  background: #fee;
  color: #c00;
  padding: 1rem;
  border-radius: 6px;
  border: 2px solid #fcc;
  font-weight: 500;
}

@media (max-width: 768px) {
  .main-content.layout-split {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    align-items: stretch;
  }

  .tab-switcher {
    flex-wrap: wrap;
  }

  .tab-switcher button {
    flex: 1;
    min-width: 100px;
  }
}
</style>