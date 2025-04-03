import ThemeSwitcher from "@src/components/ui/ThemeSwitcher";
import { Link } from "react-router-dom";

const MainFooter = () => {
  return (
    <footer className="flex justify-center items-center gap-2 dark:bg-gray-800 bg-gray-200 py-6">
      <span className="text-lg text-center block">
        <span>© </span>
        <Link to="/">react-url-shortener</Link> {new Date().getFullYear()}, All rights reserved.
      </span>
      <ThemeSwitcher />
    </footer>
  );
};

export default MainFooter;
