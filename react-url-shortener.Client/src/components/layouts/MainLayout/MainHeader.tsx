import Exit from "@src/components/svgr/Exit";
import ButtonLink from "@src/components/ui/Buttons/ButtonLink";
import SwgButton from "@src/components/ui/Buttons/SwgButton";
import CustomLink from "@src/components/ui/Links/CustomLink";
import { useAuth } from "@src/hooks/useAuth";
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const MainHeader = () => {
  const { user, isUserLoggedIn, logoutUser } = useAuth();

  const [isCollapseMenuOpen, setIsMenuOpen] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  function isLinkActive(path: string) {
    return location.pathname === path;
  };

  function toggleMenuOpen() {
    setIsMenuOpen(!isCollapseMenuOpen)
  }

  const handleLogoutClick = () => {
    logoutUser().then(() => navigate("/signup"));
  };

  return (
    <header className="flex fixed shadow-lg p-4 bg-gray-800 w-full z-50">
      <div className="flex flex-wrap items-center gap-5 w-full max-w-screen-xl mx-auto">
        <div
          id="collapseMenu"
          className={`lg:flex mx-auto ${isCollapseMenuOpen ? "flex justify-center" : "hidden"}`}
        >
          <ul className="lg:flex gap-4 max-lg:space-y-3 max-lg:fixed bg-gray-800 max-lg:w-3/4 max-lg:top-0 max-lg:h-full max-lg:left-0 max-lg:p-6 z-50">
            <li className="max-lg:py-3 px-3">
              <CustomLink onClick={toggleMenuOpen} variant={isLinkActive("/urls") ? "primary" : "secondary"} to="/urls">
                Urls
              </CustomLink>
            </li>
            <li className="max-lg:py-3 px-3">
              <CustomLink onClick={toggleMenuOpen} variant={isLinkActive("/") ? "primary" : "secondary"} to="/">
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
              <SwgButton onClick={handleLogoutClick}>
                <Exit fill="white" cursor="pointer" />
              </SwgButton>
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
            onClick={toggleMenuOpen}
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
