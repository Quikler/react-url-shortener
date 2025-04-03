import { JSX } from "react";
import CheckIcon from "../svgr/CheckIcon";
import Close from "../svgr/Close";
import ErrorIcon from "../svgr/ErrorIcon";
import WarningIcon from "../svgr/WarningIcon";

export type ToastType = "success" | "danger" | "warning";

export type ToastProps = {
  message?: string | null;
  type: ToastType;
  isVisible: boolean;
  onClose: () => void;
  transitionDuration: number;
  onVisibilityChange?: (isVisible: boolean) => void;
};

const GetToastIcon = (type: ToastType): JSX.Element => {
  switch (type) {
    case "success":
      return <CheckIcon className="dark:fill-gray-200 fill-gray-800"/>;
    case "danger":
      return <ErrorIcon className="dark:fill-gray-200 fill-gray-800"/>;
    case "warning":
      return <WarningIcon className="dark:fill-gray-200 fill-gray-800"/>;
  }
};

const Toast = ({
  type,
  message,
  isVisible,
  onClose,
  transitionDuration = 1000,
  onVisibilityChange,
}: ToastProps) => (
  <div onTransitionEnd={() => onVisibilityChange?.(isVisible)} style={{ transitionDuration: `${transitionDuration}ms`, opacity: isVisible ? 1 : 0 }} className="bg-gradient-to-r dark:from-gray-700 dark:to-gray-900 from-gray-200 to-gray-400 z-[999] flex items-center w-full max-w-xs p-4 rounded-lg shadow-sm transition-opacity"
    role="alert"
  >
    {GetToastIcon(type)}
    <div className="ms-3 mr-4 text-sm font-normal break-words">{message}</div>
    <button
      onClick={onClose}
      type="button"
      className="ms-auto -mx-1.5 -my-1.5 rounded-lg p-1.5 inline-flex items-center justify-center h-8 w-8 text-gray-500 hover: cursor-pointer"
      aria-label="Close"
    >
      <Close stroke="2" className="dark:fill-gray-200 fill-gray-800" />
    </button>
  </div>
);

export default Toast;
