import { defineConfig, loadEnv } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '')

    return {
        plugins: [plugin()],
        server: {
            port: env.PORT ? parseInt(env.PORT) : 3000,

            host: true,
            strictPort: true,
        },
        define: {
            __APP_ENV__: JSON.stringify(env.APP_ENV),
        },
    }
})