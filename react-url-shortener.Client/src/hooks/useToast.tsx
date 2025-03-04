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

  const timeoutRef = useRef<NodeJS.Timeout | null>(null);
  const handleClose = () => timeoutRef.current && clearTimeout(timeoutRef.current);

  const setToast = (msg: string, type: ToastType) => {
    timeoutRef.current && clearTimeout(timeoutRef.current);
    setIsVisible(true);
    setMessage(msg);
    setType(type);
    timeoutRef.current = setTimeout(() => setIsVisible(false), 2000);
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
      <Toast
        onClose={handleClose}
        isVisible={isVisible}
        setIsVisible={setIsVisible}
        type={type}
        message={message}
      />
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
