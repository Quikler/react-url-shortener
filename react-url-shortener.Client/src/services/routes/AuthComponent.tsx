import { useAuth } from "@src/hooks/useAuth";
import React from "react";

type Props = { children: React.ReactNode };

const AuthComponent = ({ children }: Props) => {
  const { isUserLoggedIn } = useAuth();

  return isUserLoggedIn() && children;
};

export default AuthComponent;
