import Exit from "@src/components/svgr/Exit";
import ButtonLink from "@src/components/ui/ButtonLink";
import CustomLink from "@src/components/ui/CustomLink";
import { useAuth } from "@src/hooks/useAuth";
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const MainHeader = () => {
  const { user, isUserLoggedIn, logoutUser } = useAuth();

  const [isCollapseMenuOpen, setIsMenuOpen] = useState(false);

  const navigate = useNavigate();

  const isLinkActive = (path: string) => {
    const location = useLocation();
    return location.pathname === path;
  };

  const handleLogoutClick = () => {
    logoutUser().then(() => navigate("/signup"));
  };

  return (
    <header className="flex fixed shadow-lg p-4 bg-white w-full z-50">
      <div className="flex flex-wrap items-center gap-5 w-full max-w-screen-xl mx-auto">
        <div
          id="collapseMenu"
          className={`lg:flex mx-auto ${isCollapseMenuOpen ? "flex justify-center" : "hidden"}`}
        >
          <ul className="lg:flex gap-4 max-lg:space-y-3 max-lg:fixed bg-white max-lg:w-3/4 max-lg:top-0 max-lg:h-full max-lg:left-0 max-lg:p-6  z-50">
            <li className="max-lg:border-b border-gray-400 max-lg:py-3 px-3">
              <CustomLink variant={isLinkActive("/") ? "primary" : "secondary"} to="/">
                Urls
              </CustomLink>
            </li>
            <li className="max-lg:border-b border-gray-400 max-lg:py-3 px-3">
              <CustomLink variant={isLinkActive("/about") ? "primary" : "secondary"} to="/about">
                About
              </CustomLink>
            </li>
          </ul>
        </div>
        <div className="flex items-center max-lg:ml-auto space-x-4">
          {isUserLoggedIn() ? (
            <>
              <div className="text-white bg-blue-500 rounded-full py-2 px-3">
                <p>Hello, {user?.username}</p>
              </div>
              <button onClick={handleLogoutClick}>
                <Exit cursor="pointer" />
              </button>
            </>
          ) : (
            <>
              <ButtonLink to="/signup">Sign up</ButtonLink>
              <ButtonLink variant="secondary" to="/login">
                Log in
              </ButtonLink>
            </>
          )}

          <button
            onClick={() => setIsMenuOpen(!isCollapseMenuOpen)}
            id="toggleOpen"
            className="lg:hidden"
          >
            <svg
              className="w-7 h-7"
              fill="#333"
              viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                fillRule="evenodd"
                d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                clipRule="evenodd"
              />
            </svg>
          </button>
        </div>
      </div>
    </header>
  );
};

export default MainHeader;
