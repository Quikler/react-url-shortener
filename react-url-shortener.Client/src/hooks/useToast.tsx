import React, { useContext, useEffect, useState } from "react";
import Toast, { ToastProps, ToastType } from "@src/components/ui/Toast";
import { generateUUID } from "@src/utils/helpers";

type ToastContextType = {
  success: (msg: string) => void;
  danger: (msg: string) => void;
  warning: (msg: string) => void;
};

const ToastContext = React.createContext({} as ToastContextType);

type ToastContextProviderProps = { children: React.ReactNode };

type ToastPropsExtended = ToastProps & {
  toastId: string;
  completelyInvisible: boolean;
};

export const ToastContextProvider = ({ children }: ToastContextProviderProps) => {
  console.count("Render");
  const [toastsProps, setToastsProps] = useState<ToastPropsExtended[]>([]);

  const setToast = (msg: string, type: ToastType) => {
    const toastId = generateUUID();
    console.log("Setting toast:", toastId);
    const newToastProps: ToastPropsExtended = {
      message: msg,
      type: type,
      isVisible: false,
      onClose: () => { },
      toastId: toastId,
      completelyInvisible: false,
    };

    // Add the new toast
    setToastsProps(prevToasts => [...prevToasts, newToastProps]);

    // Make it visible shortly after
    setTimeout(() => {
      console.log("Visibility true on toast:", toastId);
      setToastsProps(prevToasts =>
        prevToasts.map(t =>
          t.toastId === toastId ? { ...t, isVisible: true } : t
        )
      );
    }, 10);

    // Hide toast after 2 seconds
    setTimeout(() => {
      console.log("Visibility false on toast:", toastId);
      setToastsProps(prevToasts =>
        prevToasts.map(t =>
          t.toastId === toastId ? { ...t, isVisible: false } : t
        )
      );

      setTimeout(() => {
        console.log("Transition pass on toast (completely invisible):", toastId);
        setToastsProps(prevToasts =>
          prevToasts.map(t =>
            t.toastId === toastId ? { ...t, completelyInvisible: true } : t
          )
        );
      }, 1000); // Matches transition duration
    }, 2000);
  };

  useEffect(() => {
    if (toastsProps.length === 0) {
      return;
    }
    if (toastsProps.every(t => t.completelyInvisible === true)) {
      setToastsProps([]);
    }
  }, [toastsProps])

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
      {toastsProps.length !== 0 && <ul className="fixed top-24 right-12 flex flex-col gap-2">
        {toastsProps.map((value, index) => {
          return <li key={index}><Toast type={value.type} message={value.message} isVisible={value.isVisible} onClose={value.onClose} /></li>
        })}
      </ul>}
      {children}
    </ToastContext.Provider>
  )
};

export const useToast = () => {
  const toastContext = useContext(ToastContext);

  if (!toastContext) {
    throw new Error("useToast must be used within a ToastContext");
  }

  return toastContext;
};
