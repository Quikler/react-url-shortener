import { Link } from "react-router-dom";

const MainFooter = () => {
  return (
    <footer className="bg-white mx-auto py-6 font-[sans-serif]">
      <span className="text-lg text-gray-500 text-center block">
        <span>© </span>
        <Link to="/">react-url-shortener</Link> 2025, All rights reserved.
      </span>
    </footer>
  );
};

export default MainFooter;
