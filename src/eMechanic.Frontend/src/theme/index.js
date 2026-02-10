import { createSystem, defaultConfig, defineConfig } from "@chakra-ui/react";

const config = defineConfig({
  theme: {
    tokens: {
      colors: {
        brand: {
          50: { value: "#eff6ff" },
          100: { value: "#dbeafe" },
          300: {value: "#569af7"},
          500: { value: "#3b82f6" },
          600: { value: "#2563EB" }, 
          700: { value: "#1d4ed8" }, 
        },
      },
    },
    semanticTokens: {
      colors: {
        brand: {
          solid: { value: "#2563EB" },    
          contrast: { value: "#FFFFFF" }, 
          fg: { value: "#2563EB" },       
          muted: { value: "#eff6ff" },    
        },
      },
    },
  },
});

export const system = createSystem(defaultConfig, config);