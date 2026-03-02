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
  gap: 0.75rem;
  padding: 0.75rem 0.75rem 0.5rem;
  max-height: 70vh;
  overflow-y: auto;
  background: #f3f4f6;
  color: #000000;
  border-radius: 0.5rem;
}

/* Individual message rows */

.message {
  display: flex;
  animation: fadeIn 0.2s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(4px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.message-wrapper {
  max-width: 72%;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
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

/* Labels */

.message-label {
  font-size: 0.7rem;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

/* Bubbles */
.message-bubble {
  padding: 0.6rem 0.85rem;
  border-radius: 1rem;
  font-size: 0.9rem;
  line-height: 1.5;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.06);
  word-wrap: break-word;
  background: #e73a3a;
  color: #111827;  
}

/* Assistant bubble: soft neutral */

.assistant-wrapper .message-bubble {
  background: #e5e7eb;
  color: #111827; 
  border-bottom-left-radius: 0.4rem;
}

/* User bubble: solid accent, but not neon */

.user-wrapper .message-bubble {
  background: #ffffff;
  color: #0e0d0d; 
  border-bottom-right-radius: 0.4rem;
}

/* Error bubble */

.error-wrapper .message-bubble {
  background: #ececec;
  color: #b91c1c;
  border-radius: 0.75rem;
  border: 1px solid #fecaca;
}

/* Markdown content in assistant messages */

.message-bubble :deep(h1),
.message-bubble :deep(h2),
.message-bubble :deep(h3) {
  margin-top: 0.35rem;
  margin-bottom: 0.2rem;
  font-weight: 600;
  letter-spacing: -0.01em;
}

.message-bubble :deep(p) {
  margin: 0.1rem 0 0.25rem;
}

.message-bubble :deep(ul),
.message-bubble :deep(ol) {
  margin: 0.2rem 0 0.25rem;
  padding-left: 1.15rem;
}

.message-bubble :deep(li) {
  margin: 0.1rem 0;
}

.message-bubble :deep(code) {
  background: rgba(15, 23, 42, 0.06);
  padding: 0.05rem 0.3rem;
  border-radius: 0.25rem;
  font-family: "SF Mono", ui-monospace, Menlo, Monaco, Consolas, "Liberation Mono",
    "Courier New", monospace;
  font-size: 0.82rem;
}

.message-bubble :deep(pre) {
  background: #111827;
  color: #000000;
  padding: 0.6rem 0.75rem;
  border-radius: 0.5rem;
  overflow-x: auto;
  margin: 0.35rem 0 0.25rem;
  font-size: 0.82rem;
}

.message-bubble :deep(strong) {
  font-weight: 600;
}

.message-bubble :deep(em) {
  font-style: italic;
}

/* Thinking indicator */
.thinking-dialog {
  display: flex;
  align-self: flex-start;
  max-width: 72%;
  animation: fadeIn 0.2s ease-out;
}

.thinking-wrapper {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.55rem 0.8rem;
  background: #e5e7eb;
  border-radius: 1rem;
  border-bottom-left-radius: 0.4rem;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.06);
}

.thinking-spinner {
  width: 14px;
  height: 14px;
  border: 2px solid #9ca3af;
  border-top-color: transparent;
  border-radius: 999px;
  animation: spin 0.75s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.thinking-text {
  font-size: 0.85rem;
  color: #6b7280;
  font-style: italic;
}

/* Mobile tweaks */

@media (max-width: 640px) {
  .message-wrapper,
  .thinking-dialog {
    max-width: 100%;
  }
}
</style>