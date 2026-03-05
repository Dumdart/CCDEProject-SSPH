import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import { ApiRepository } from './repository';
import { Services } from './service';
import type { InjectionKey } from 'vue';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
const apiKey = import.meta.env.VITE_API_KEY;

const apiRepo = new ApiRepository(apiBaseUrl, apiKey);
const services = new Services(apiRepo)

const app = createApp(App);

const servicesKey: InjectionKey<Services> = Symbol("services");
const apiRepoKey: InjectionKey<ApiRepository> = Symbol("apiRepo");
app.provide(servicesKey, services);
app.provide(apiRepoKey, apiRepo);

app.mount('#app');

export { servicesKey, apiRepoKey};
