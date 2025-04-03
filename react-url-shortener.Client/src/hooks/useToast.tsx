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
  transformableMultiplyer: number;
  toastIndex: number;
};

export const ToastContextProvider = ({ children }: ToastContextProviderProps) => {
  const TOAST_TRANSITION_DURATION = 1000;
  const TOAST_DISPLAY_LIFETIME = 2000;

  const [toastsProps, setToastsProps] = useState<ToastPropsExtended[]>([]);

  const setToast = (msg: string, type: ToastType) => {
    const toastId = generateUUID();
    const toastIndex = toastsProps.length - 1;

    const newToastProps: ToastPropsExtended = {
      message: msg,
      type: type,
      isVisible: false,
      onClose: () => toggleIsVisible(toastId, false),
      transitionDuration: TOAST_TRANSITION_DURATION,
      toastId: toastId,
      completelyInvisible: false,
      transformableMultiplyer: 0,
      toastIndex: toastIndex,
      onVisibilityChange: (isVisible: boolean) => {
        if (isVisible) {
          setTimeout(() => toggleIsVisible(toastId, false), TOAST_DISPLAY_LIFETIME);
        } else {
          setToastsProps(prevToasts => {
            return prevToasts.map((t) => {
              if (t.toastId === toastId) {
                return { ...t, completelyInvisible: true };
              } else {
                return { ...t, transformableMultiplyer: t.transformableMultiplyer === 0 ? 1 : t.transformableMultiplyer + 1, }
              }
            })
          });
        }
      }
    };

    // Add the new toast
    setToastsProps(prevToasts => {
      if (prevToasts.length > 0) {
        const previousToast = prevToasts[prevToasts.length - 1];
        newToastProps.transformableMultiplyer = previousToast.transformableMultiplyer;
      }
      return [...prevToasts, newToastProps];
    });

    // Make it visible shortly after
    setTimeout(() => toggleIsVisible(toastId, true), 10);
  };

  function toggleIsVisible(toastId: string, visibility: boolean) {
    setToastsProps(prevToasts =>
      prevToasts.map(t =>
        t.toastId === toastId ? { ...t, isVisible: visibility } : t
      )
    );
  }

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
      {toastsProps.length !== 0 &&
        <ul className="fixed top-24 right-12 flex flex-col gap-2">
          {toastsProps.map((value) => (
            (
              <li key={value.toastId} className="transition-transform duration-300 ease-out" style={{ transform: `translateY(${-60 * value.transformableMultiplyer}px)` }}>
                <Toast
                  type={value.type}
                  message={value.message}
                  isVisible={value.isVisible}
                  onClose={value.onClose}
                  transitionDuration={value.transitionDuration}
                  onVisibilityChange={value.onVisibilityChange}
                />
              </li>
            )
          ))}
        </ul>
      }
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
