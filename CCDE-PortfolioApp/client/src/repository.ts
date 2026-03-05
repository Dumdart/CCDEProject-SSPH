import type { AnalyticsResponse, ChatRequest, ChatResponse } from "./types";

export class ApiRepository {
  private apiBaseUrl: string;
  private apiKey: string;

  constructor(apiBaseUrl: string, apiKey: string) {
    this.apiBaseUrl = apiBaseUrl;
    this.apiKey = apiKey;
  }

  async fetchAnalytics(pageId: string): Promise<AnalyticsResponse> {
    const res = await fetch(
      `${this.apiBaseUrl}/api/pageview/visitor-count?pageId=${pageId}&code=${this.apiKey}`
    );
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }
    return await res.json() as AnalyticsResponse;
  }

  async sendChatMessage(request: ChatRequest): Promise<ChatResponse> {
    const res = await fetch(`${this.apiBaseUrl}/api/llm-chat/chat?code=${this.apiKey}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }
    return await res.json() as ChatResponse;
  }

  async analyzeDocument(content: string | ArrayBuffer , filename: string): Promise<string> {
    const prompt = `Please analyze this document:

Filename: ${filename}
Content:
${content}`;
    const request: ChatRequest = { Message: prompt };
    const response = await this.sendChatMessage(request);
    return response.Response ?? "No analysis available";
  }
}