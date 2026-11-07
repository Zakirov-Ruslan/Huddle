# Huddle — React client

## Layout & responsiveness

The chat is a three-column layout: **server rail → channel list → channel content**.

| Screen | Width | Behaviour |
| --- | --- | --- |
| Phone | `< 768px` (`md`) | Two screens: channel list (`/s/:serverId`) **or** channel (`/s/:serverId/ch/:channelId`). The back arrow in the channel header returns to the list. |
| Tablet | `768px – 1023px` | Three columns, narrow rail (`4rem`) and channel list (`15rem`). |
| Desktop | `≥ 1024px` (`lg`) | Three columns, rail `6rem`, channel list `300px`. |

Key rules:

- The layout is built with **flex**, not CSS grid. Every container that scrolls has `min-h-0`, so nested
  flex columns scroll correctly instead of overflowing.
- On phones the "which screen is visible" state comes **from the URL**, not from a store flag. That is why
  the browser back button and the Android back gesture work for free. Don't add a separate
  `isChatOpened` state — see `pages/Server.tsx` (`isChannelOpen`) and `DefaultChannelRedirect.tsx`.
- `hooks/useIsMobile.ts` is only for cases where *behaviour* (not markup) depends on the viewport.
  For pure layout use Tailwind variants (`max-md:hidden`, `md:flex`, ...).
- The members list (`pages/Channel/Channel.tsx`) is an overlay on phones and a column from `md` up.

## Styling (Tailwind)

`tailwind.output.css` is generated and committed; `index.html` links it directly.

```bash
npm run css:build   # regenerate tailwind.output.css (also runs before `dev` and `build`)
npm run css:watch   # regenerate on every change
```

New utility classes will not appear until this CSS is rebuilt.

---

# React + TypeScript + Vite


This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      ...tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      ...tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      ...tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
