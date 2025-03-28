import React, { useContext, useRef, useState } from "react";
import Toast, { ToastType } from "@src/components/ui/Toast";

type ToastContextType = {
  success: (msg: string) => void;
  danger: (msg: string) => void;
  warning: (msg: string) => void;
};

const ToastContext = React.createContext({} as ToastContextType);

type ToastContextProviderProps = { children: React.ReactNode };

export const ToastContextProvider = ({ children }: ToastContextProviderProps) => {
  const [message, setMessage] = useState<string | null>(null);
  const [type, setType] = useState<ToastType>("success");
  const [isVisible, setIsVisible] = useState(false);
  const [isToastInDOM, setIsToastInDOM] = useState(false);

  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const handleClose = () => {
    setIsVisible(false);
    timeoutRef.current && clearTimeout(timeoutRef.current);
    timeoutRef.current = setTimeout(() => {
      setIsVisible(false);
      // Wait for the transition duration before removing from DOM
      setTimeout(() => setIsToastInDOM(false), 1000); // 1000ms to match the transition duration
    }, 2000); // Display duration of the toast
  }

  const setToast = (msg: string, type: ToastType) => {
    timeoutRef.current && clearTimeout(timeoutRef.current);
    setIsToastInDOM(true);
    setMessage(msg);
    setType(type);

    //setIsVisible(true);
    setTimeout(() => setIsVisible(true), 10);

    timeoutRef.current = setTimeout(() => {
      setIsVisible(false);
      // Wait for the transition duration before removing from DOM
      setTimeout(() => setIsToastInDOM(false), 1000); // 1000ms to match the transition duration
    }, 2000); // Display duration of the toast
  };

  const success = (msg: string) => setToast(msg, "success");
  const danger = (msg: string) => setToast(msg, "danger");
  const warning = (msg: string) => setToast(msg, "warning");

  const value = {
    success,
    warning,
    danger,
  };

  return (
    <ToastContext.Provider value={value}>
      {isToastInDOM && <Toast
        onClose={handleClose}
        isVisible={isVisible}
        type={type}
        message={message}
      />}
      {children}
    </ToastContext.Provider>
  );
};

export const useToast = () => {
  const toastContext = useContext(ToastContext);

  if (!toastContext) {
    throw new Error("useToast must be used within a ToastContext");
  }

  return toastContext;
};
