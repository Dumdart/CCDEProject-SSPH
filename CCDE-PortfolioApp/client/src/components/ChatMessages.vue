<template>
  <div class="chat-messages">
    <div
      v-for="(msg, idx) in messages"
      :key="idx"
      :class="['message', `message-${msg.role}`]"
    >
      <!-- Error messages: centered red -->
      <div v-if="msg.role === 'error'" class="message-wrapper error-wrapper">
        <div class="message-bubble">{{ msg.content }}</div>
      </div>

      <!-- Assistant messages: left-aligned with label -->
      <div v-else-if="msg.role === 'assistant'" class="message-wrapper assistant-wrapper">
        <div class="message-label">Assistant</div>
        <div class="message-bubble" v-html="renderMarkdown(msg.content)"></div>
      </div>

      <!-- User messages: right-aligned plain bubble -->
      <div v-else-if="msg.role === 'user'" class="message-wrapper user-wrapper">
        <div class="message-bubble">{{ msg.content }}</div>
      </div>

      <!-- Fallback for unexpected roles -->
      <div v-else class="message-wrapper">
        <div class="message-bubble">{{ msg.content }}</div>
      </div>
    </div>

    <!-- Thinking Dialog Component -->
    <div v-if="isThinking" class="thinking-dialog">
      <div class="thinking-wrapper">
        <div class="thinking-spinner"></div>
        <div class="thinking-text">AI is thinking...</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { marked } from 'marked';
import DOMPurify from 'dompurify';
import type { Message } from '../types';

interface Props {
  messages: Message[];
  isThinking?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  messages: () => [],
  isThinking: false
});

// Configure marked for proper rendering
marked.setOptions({
  breaks: true,
  gfm: true,
});

function renderMarkdown(content: string): string {
  const rawHtml = marked.parse(content) as string;
  console.log(rawHtml);
  return DOMPurify.sanitize(rawHtml);
}
</script>

<style scoped>
.chat-messages {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 16px;
  max-height: 80vh;
  overflow-y: auto;
  background: #f8f9fa;
}

.message {
  display: flex;
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.message-wrapper {
  max-width: 75%;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.assistant-wrapper {
  align-self: flex-start;
}

.user-wrapper {
  align-self: flex-end;
}

.error-wrapper {
  align-self: center;
  max-width: 90%;
}

.message-label {
  font-size: 12px;
  font-weight: 600;
  color: #666;
  align-self: flex-start;
}

.message-bubble {
  padding: 12px 16px;
  border-radius: 18px;
  font-size: 14px;
  line-height: 1.4;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
  word-wrap: break-word;
}

/* Markdown content styling */
.message-bubble :deep(h1),
.message-bubble :deep(h2),
.message-bubble :deep(h3) {
  margin-top: 16px;
  margin-bottom: 8px;
  font-weight: 600;
}

.message-bubble :deep(ul),
.message-bubble :deep(ol) {
  margin: 8px 0;
  padding-left: 24px;
}

.message-bubble :deep(code) {
  background: rgba(0, 0, 0, 0.1);
  padding: 2px 6px;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
}

.message-bubble :deep(pre) {
  background: rgba(0, 0, 0, 0.05);
  padding: 12px;
  border-radius: 6px;
  overflow-x: auto;
  margin: 8px 0;
}

.message-bubble :deep(strong) {
  font-weight: 700;
}

.message-bubble :deep(em) {
  font-style: italic;
}

.assistant-wrapper .message-bubble {
  background: #e5e5ea;
  color: #000;
  border-bottom-left-radius: 4px;
}

.user-wrapper .message-bubble {
  background: linear-gradient(135deg, #007aff, #0056cc);
  color: white;
  border-bottom-right-radius: 4px;
}

.error-wrapper .message-bubble {
  background: #fee;
  color: #c33;
  border: 1px solid #fcc;
  border-radius: 8px;
}

/* Thinking Dialog Styles */
.thinking-dialog {
  display: flex;
  align-self: flex-start;
  max-width: 75%;
  animation: fadeIn 0.3s ease-in;
}

.thinking-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  background: #e5e5ea;
  border-radius: 18px;
  border-bottom-left-radius: 4px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.thinking-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid #999;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.thinking-text {
  font-size: 14px;
  color: #666;
  font-style: italic;
}
</style>
