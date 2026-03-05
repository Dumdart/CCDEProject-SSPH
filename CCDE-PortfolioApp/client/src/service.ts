import { inject } from "vue";
import type { ApiRepository } from "./repository";
import type { ChatRequest, ChatResponse, Message } from "./types";

export class Services {
    private readonly repo : ApiRepository

    constructor(repo: ApiRepository) {
        this.repo = repo;
    }

    public async sendChatMessage(
        messageText: string,
        chat: Message[],
        setLoading: (v: boolean) => void,
        setError: (v: string) => void,
        context?: string
    ): Promise<void> {
        if (!messageText.trim()) return;

        const userMsg = messageText.trim();
        chat.push({ role: "user", content: userMsg });

        const fullPrompt = context
            ? `${context}\n\nUser question:\n${userMsg}`
            : userMsg;

        setLoading(true);
        setError("");

        try {
            const request: ChatRequest = { Message: fullPrompt };
            const data: ChatResponse = await this.repo.sendChatMessage(request);

            chat.push({
                role: "assistant",
                content: data.Response ?? "No response from AI",
            });
        } catch (e: any) {
            const msg = e?.message ?? "Unknown error";
            setError(msg);
            chat.push({
                role: "error",
                content: `Error: ${msg}`,
            });
        } finally {
            setLoading(false);
        }
    }
}