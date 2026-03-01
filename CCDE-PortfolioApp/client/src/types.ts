export interface AnalyticsResponse {
  ViewCount: number;
  LastUpdated: string;
  Id: string;
  PageId: string;
}

export interface ChatRequest {
  Message: string;
}

export interface ChatResponse {
  Response: string;
}

export interface Message {
  role: 'user' | 'assistant' | 'error';
  content: string;
}
export interface UploadedFile {
  name: string;
  size: number;
  type: string;
  content: string | ArrayBuffer | null;
}