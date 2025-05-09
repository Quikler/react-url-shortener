import App from "@src/App";
import MainLayout from "@src/components/layouts/MainLayout/MainLayout";
import { AuthProvider } from "@src/hooks/useAuth";
import { ToastContextProvider } from "@src/hooks/useToast";
import AboutPage from "@src/pages/about/AboutPage";
import LoginPage from "@src/pages/login/LoginPage";
import SignupPage from "@src/pages/signup/SignupPage";
import UrlsMain from "@src/pages/urls/UrlsMain";
import UrlsPage from "@src/pages/urls/UrlsPage";
import UrlInfoPage from "@src/pages/urls/{urlId}/UrlInfoPage";
import AuthRoute from "./AuthRoute";

const routerConfig = [
  {
    path: "/",
    element: (
      <AuthProvider>
        <MainLayout>
          <App />

          { /* Toast context for displaying toasts (for instance, toast.warning("test")) */}
          <ToastContextProvider /> 
        </MainLayout>
      </AuthProvider>
    ),
    children: [
      {
        path: "urls",
        element: <UrlsPage />,
        children: [
          {
            index: true,
            element: <UrlsMain />,
          },
          {
            path: ":urlId/info",
            element: (
              <AuthRoute>
                <UrlInfoPage />
              </AuthRoute>
            ),
          },
        ],
      },
      {
        path: "urls",
        element: <UrlsPage />,
        children: [
          { path: "", index: true, element: <UrlsMain /> },
          {
            path: ":urlId/info",
            element: (
              <AuthRoute>
                <UrlInfoPage />
              </AuthRoute>
            ),
          },
        ],
      },
      {
        path: "login",
        element: <LoginPage />,
      },
      {
        path: "signup",
        element: <SignupPage />,
      },
      {
        index: true,
        element: <AboutPage />,
      },
    ],
  },
];

export default routerConfig;
