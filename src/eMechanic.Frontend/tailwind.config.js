/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#2563EB', // Vibrant Blue
          hover: '#1D4ED8',
          foreground: '#FFFFFF',
        },
        secondary: {
          DEFAULT: '#F97316', // Vibrant Orange
          hover: '#EA580C',
          foreground: '#FFFFFF',
        },
        dark: {
          DEFAULT: '#0F172A', // Slate 900
          lighter: '#1E293B', // Slate 800
          card: 'rgba(30, 41, 59, 0.7)', // Glassmorphism base
        },
        light: {
          DEFAULT: '#F8FAFC', // Slate 50
          muted: '#E2E8F0', // Slate 200
        }
      },
      fontFamily: {
        sans: ['Inter', 'Plus Jakarta Sans', 'sans-serif'],
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'hero-pattern': "url('/src/assets/grid.svg')", // Placeholder for now
      }
    },
  },
  plugins: [],

  // --- TUTAJ DODAJESZ TĘ ZMIANĘ ---
  corePlugins: {
    preflight: false, // To wyłącza reset stylów, który psuje Chakrę
  },
  // --------------------------------
}