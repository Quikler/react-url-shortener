import ThemeSwitcher from "@src/components/ui/ThemeSwitcher";
import { Link } from "react-router-dom";

const MainFooter = () => {
  return (
    <footer className="flex justify-center items-center gap-2 dark:bg-gray-800 bg-white py-6">
      <span className="text-lg text-gray-500 dark:text-white text-center block">
        <span>© </span>
        <Link to="/">react-url-shortener</Link> 2025, All rights reserved.
      </span>
      <ThemeSwitcher />
    </footer>
  );
};

export default MainFooter;
