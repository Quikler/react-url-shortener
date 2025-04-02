import React, { useLayoutEffect, useState } from "react"
import Computer from "../svgr/Computer"
import Moon from "../svgr/Moon"
import Sun from "../svgr/Sun"
import SwgButton from "./Buttons/SwgButton"

const ThemeSwitcher = () => {
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'system');
  const [isDark, setIsDark] = useState(checkIfDark());

  useLayoutEffect(() => {
    if (theme === 'system') {
      document.documentElement.classList.toggle("dark", checkIfDark());
      setIsDark(checkIfDark());
    } else if (theme === 'light') {
      localStorage.setItem('theme', theme);
      document.documentElement.classList.remove('dark');
      setIsDark(false);
    } else {
      localStorage.setItem('theme', theme);
      document.documentElement.classList.add('dark');
      setIsDark(true);
    }
  }, [theme]);

  function checkIfDark() {
    return localStorage.theme === "dark" || (!("theme" in localStorage) && window.matchMedia("(prefers-color-scheme: dark)").matches)
  }

  function toggleTheme(e: React.MouseEvent<HTMLButtonElement>) {
    if (e.currentTarget.id === 'system') {
      setTheme('system');
      localStorage.removeItem('theme');
    } else if (e.currentTarget.id === 'light') {
      setTheme('light');
    } else {
      setTheme('dark');
    }
  }

  return <div className="flex gap-2 items-center">
    <SwgButton className="cursor-pointer" id="system" onClick={toggleTheme}>
      <Computer className={isDark ? 'fill-gray-200' : 'fill-gray-800'} />
    </SwgButton>
    <SwgButton className="cursor-pointer" id="light" onClick={toggleTheme}>
      <Sun className={isDark ? 'fill-gray-200' : 'fill-gray-800'}/>
    </SwgButton>
    <SwgButton className="cursor-pointer" id="dark" onClick={toggleTheme}>
      <Moon className={isDark ? 'fill-gray-200' : 'fill-gray-800'}/>
    </SwgButton>
  </div>
}

export default ThemeSwitcher;
