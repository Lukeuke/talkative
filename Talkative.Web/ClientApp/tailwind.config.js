/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,js,jsx}"],
  theme: {
    extend: {
      colors: {
        MainDark: '#27272D',
        SemiDark: '#313137',
        Main: '#3B3B41',
        MainSemiLight: '#45454B',
        MainLight: '#4F4F55'
      },
    },
  },
  plugins: [],
}

