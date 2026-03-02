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

/* Design tokens for a neutral, professional look */
.app-container {
  --bg-body: #f4f5f7;
  --bg-surface: #ffffff;
  --bg-subtle: #f3f4f6;
  --bg-soft-accent: #eef2ff;

  --border-subtle: #e5e7eb;
  --border-strong: #d1d5db;

  --accent: #2563eb;      /* Primary accent (buttons, active tabs) */
  --accent-dark: #1d4ed8; /* Hover state */
  --accent-soft-text: #1f2937;

  --text-main: #111827;
  --text-muted: #6b7280;
  --text-soft: #9ca3af;
  --text-danger: #b91c1c;

  --danger: #dc2626;
  --danger-dark: #b91c1c;

  --radius-sm: 0.375rem;
  --radius-md: 0.5rem;
  --radius-lg: 0.75rem;

  --shadow-sm: 0 1px 2px rgba(15, 23, 42, 0.05);
  --shadow-md: 0 4px 12px rgba(15, 23, 42, 0.06);

  --transition-fast: 150ms ease-out;

  min-height: 100vh;
  background: var(--bg-body);
  font-family: -apple-system, BlinkMacSystemFont, system-ui, -system-ui,
    "Segoe UI", Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  color: var(--text-main);
}

/* Header / Nav */

.header {
  background: var(--bg-surface);
  padding: 1.25rem 2rem;
  box-shadow: var(--shadow-sm);
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  border-bottom: 1px solid var(--border-subtle);
}

.header h1 {
  margin: 0;
  font-size: 1.4rem;
  font-weight: 600;
  letter-spacing: -0.01em;
  color: var(--text-main);
}

.tab-switcher {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

/* Tab buttons */

.tab-switcher button {
  position: relative;
  padding: 0.5rem 1rem;
  border-radius: 999px;
  border: 1px solid transparent;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  line-height: 1.4;
  transition: background-color var(--transition-fast),
    color var(--transition-fast), border-color var(--transition-fast),
    box-shadow var(--transition-fast);
}

.tab-switcher button:hover {
  background: #e5e7eb;
}

.tab-switcher button.active {
  background: var(--bg-soft-accent);
  color: var(--accent-soft-text);
  border-color: #c7d2fe;
  box-shadow: 0 0 0 1px rgba(129, 140, 248, 0.5);
}

/* Layout */

.main-content {
  padding: 1.75rem 2rem 2rem;
  display: grid;
  gap: 1.5rem;
  max-width: 1280px;
  margin: 0 auto;
}

.main-content.layout-dashboard,
.main-content.layout-chat,
.main-content.layout-documents {
  grid-template-columns: 1fr;
}

.main-content.layout-split {
  grid-template-columns: minmax(0, 1.1fr) minmax(0, 1.1fr);
}

/* Panels */

.panel {
  background: var(--bg-surface);
  border-radius: var(--radius-lg);
  padding: 1.5rem 1.5rem 1.75rem;
  box-shadow: var(--shadow-md);
  border: 1px solid var(--border-subtle);
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--border-subtle);
}

.panel-header h2 {
  margin: 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--text-main);
  letter-spacing: -0.01em;
}

/* Generic buttons */

.btn-refresh,
.btn-clear,
.btn-upload,
.btn-send,
.btn-analyze {
  padding: 0.55rem 1.1rem;
  border-radius: var(--radius-md);
  border: 1px solid transparent;
  background: var(--accent);
  color: #ffffff;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  line-height: 1.3;
  transition: background-color var(--transition-fast),
    box-shadow var(--transition-fast),
    border-color var(--transition-fast),
    transform var(--transition-fast);
}

.btn-refresh:hover:not(:disabled),
.btn-clear:hover:not(:disabled),
.btn-upload:hover:not(:disabled),
.btn-send:hover:not(:disabled),
.btn-analyze:hover:not(:disabled) {
  background: var(--accent-dark);
  box-shadow: 0 4px 10px rgba(37, 99, 235, 0.35);
  transform: translateY(-1px);
}

.btn-refresh:disabled,
.btn-send:disabled,
.btn-analyze:disabled {
  opacity: 0.45;
  cursor: default;
  box-shadow: none;
}

.btn-clear {
  background: #f3f4f6;
  color: var(--text-main);
  border-color: var(--border-subtle);
}

.btn-clear:hover:not(:disabled) {
  background: #e5e7eb;
  box-shadow: var(--shadow-sm);
}

/* Upload is styled like primary, but slightly softer */
.btn-upload {
  background: var(--accent);
}

/* Analytics */

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(190px, 1fr));
  gap: 1.25rem;
}

.stat-card {
  background: var(--bg-subtle);
  border-radius: var(--radius-md);
  border: 1px solid var(--border-subtle);
  padding: 1.25rem 1rem;
  text-align: left;
  transition: border-color var(--transition-fast),
    box-shadow var(--transition-fast),
    transform var(--transition-fast),
    background-color var(--transition-fast);
}

.stat-card:hover {
  border-color: var(--border-strong);
  background-color: #eef2f7;
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
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
    transform: scale(1.02);
  }
}

.stat-value {
  font-size: 2.1rem;
  font-weight: 700;
  color: var(--accent);
  margin-bottom: 0.25rem;
  letter-spacing: -0.03em;
}

.stat-value-small {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-main);
  margin-bottom: 0.15rem;
}

.stat-label {
  font-size: 0.8rem;
  color: var(--text-soft);
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

/* Quick actions */

.quick-actions {
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid var(--border-subtle);
}

.quick-actions h3 {
  margin: 0 0 0.75rem 0;
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-main);
}

.btn-action {
  display: block;
  width: 100%;
  padding: 0.65rem 0.9rem;
  margin-bottom: 0.5rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-subtle);
  background: #ffffff;
  color: var(--accent-soft-text);
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  text-align: left;
  transition: background-color var(--transition-fast),
    border-color var(--transition-fast),
    box-shadow var(--transition-fast),
    transform var(--transition-fast);
}

.btn-action:hover {
  background: var(--bg-soft-accent);
  border-color: #c7d2fe;
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
}

/* Chat panel layout */

.chat-panel {
  max-height: 80vh;
  display: flex;
  flex-direction: column;
}

.chat-input-form {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.75rem;
}

.chat-input {
  flex: 1;
  padding: 0.7rem 0.9rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-subtle);
  font-size: 0.95rem;
  font-family: inherit;
  line-height: 1.4;
  background: #f9fafb;
  color: #000000;
  transition: border-color var(--transition-fast),
    box-shadow var(--transition-fast),
    background-color var(--transition-fast);
}

.chat-input:focus {
  outline: none;
  border-color: var(--accent);
  box-shadow: 0 0 0 1px rgba(37, 99, 235, 0.2);
  background: #ffffff;
}

/* Documents */

.upload-info {
  background: var(--bg-subtle);
  padding: 0.6rem 0.75rem;
  border-radius: var(--radius-md);
  font-size: 0.8rem;
  color: var(--text-muted);
  border: 1px solid var(--border-subtle);
}

.empty-state {
  text-align: center;
  padding: 2.5rem 1rem;
  color: var(--text-muted);
  font-size: 0.9rem;
}

.files-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.file-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0.9rem;
  background: var(--bg-subtle);
  border-radius: var(--radius-md);
  border: 1px solid var(--border-subtle);
  transition: border-color var(--transition-fast),
    box-shadow var(--transition-fast),
    background-color var(--transition-fast);
}

.file-item:hover {
  border-color: var(--border-strong);
  background: #eef2f7;
  box-shadow: var(--shadow-sm);
}

.file-info {
  flex: 1;
  min-width: 0;
}

.file-name {
  font-weight: 600;
  color: var(--text-main);
  margin-bottom: 0.1rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-meta {
  font-size: 0.8rem;
  color: var(--text-soft);
}

.file-actions {
  display: flex;
  gap: 0.4rem;
  margin-left: 0.75rem;
}

.btn-remove {
  padding: 0.5rem 0.85rem;
  border-radius: var(--radius-md);
  border: 1px solid transparent;
  background: #fee2e2;
  color: var(--danger-dark);
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 500;
  transition: background-color var(--transition-fast),
    border-color var(--transition-fast),
    box-shadow var(--transition-fast);
}

.btn-remove:hover {
  background: #fecaca;
  border-color: #fca5a5;
  box-shadow: var(--shadow-sm);
}

/* Analysis result */

.analysis-result {
  margin-top: 0.5rem;
  padding: 1.1rem 1rem;
  background: #f9fafb;
  border-radius: var(--radius-md);
  border: 1px solid #dbeafe;
}

.analysis-result h3 {
  margin: 0 0 0.5rem 0;
  color: var(--text-main);
  font-size: 0.95rem;
  font-weight: 600;
}

.result-content {
  line-height: 1.5;
  color: var(--text-main);
  white-space: pre-wrap;
  font-size: 0.9rem;
}

/* Errors */

.error-box {
  background: #fef2f2;
  color: var(--text-danger);
  padding: 0.85rem 0.9rem;
  border-radius: var(--radius-md);
  border: 1px solid #fecaca;
  font-size: 0.9rem;
}

/* Responsive */

@media (max-width: 768px) {
  .main-content.layout-split {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    align-items: flex-start;
  }

  .header h1 {
    font-size: 1.25rem;
  }

  .tab-switcher button {
    flex: 1 1 45%;
  }
}
</style>
