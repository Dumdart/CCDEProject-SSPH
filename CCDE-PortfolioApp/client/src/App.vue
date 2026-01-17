<script setup>
import { ref, onMounted, computed } from "vue";

const loading = ref(false);
const error = ref("");
const viewCount = ref(0);
const lastUpdated = ref("");

// Get gitsecret from meta env(configured in pipeline) 
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
const apiKey = import.meta.env.VITE_API_KEY;

async function refresh() {
  loading.value = true;
  error.value = "";
  try {
    // Azure Static Web Apps proxies /api to the Functions backend
    const res = await fetch(`${apiBaseUrl}/api/visitor-count?pageId=home&code=${apiKey}`);
    console.log("Status:", res.status, "OK?", res.ok);
    console.log("Headers:", [...res.headers.entries()]);

    const data = await res.json();    
    console.log("Raw body:", text);

    if (!res.ok) throw new Error(data?.message || "Request failed");

    viewCount.value = data.viewCount;
    lastUpdated.value = data.lastUpdated;
  } 
  catch (e) {
    error.value = e.message;
  } 
  finally {
    loading.value = false;
  }
}

onMounted(refresh);
</script>


<template>
  <main style="font-family: sans-serif; padding: 24px;">
    <h1>Home</h1>

    <p v-if="loading">Loading visitor count…</p>
    <p v-else-if="error">Error: {{ error }}</p>
    <p v-else>
      Views: <strong>{{ viewCount }}</strong><br />
      Last updated: {{ lastUpdated }}
    </p>

    <button @click="refresh" :disabled="loading">Refresh (increments)</button>
  </main>
</template>