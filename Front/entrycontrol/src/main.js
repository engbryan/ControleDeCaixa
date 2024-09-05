/**
 * main.js
 *
 * Bootstraps Vuetify and other plugins then mounts the App
 */

// Composables
import { createApp } from 'vue'

// Components
import App from './App.vue'

// Plugins
import { registerPlugins } from '@/plugins'

import { Buffer } from 'buffer';
window.Buffer = Buffer;

import process from 'process';
window.process = process;

const app = createApp(App)

registerPlugins(app)

app.mount('#app')
