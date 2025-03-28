import { twMerge } from "tailwind-merge";
import CheckIcon from "../svgr/CheckIcon";
import Close from "../svgr/Close";
import ErrorIcon from "../svgr/ErrorIcon";
import WarningIcon from "../svgr/WarningIcon";

export type ToastType = "success" | "danger" | "warning";

export type ToastProps = React.HTMLAttributes<HTMLDivElement> & {
  message?: string | null;
  type: ToastType;
  isVisible: boolean;
  onClose: () => void;
};

const GetToastIcon = (type: ToastType): JSX.Element => {
  switch (type) {
    case "success":
      return <CheckIcon />;
    case "danger":
      return <ErrorIcon />;
    case "warning":
      return <WarningIcon />;
  }
};

const Toast = ({
  className,
  type,
  message,
  isVisible,
  onClose,
  ...rest
}: ToastProps) => {

  return (
    <div className={twMerge(
      `z-[999] flex items-center w-full max-w-xs p-4 rounded-lg shadow-sm text-gray-400 bg-gray-800 transition-opacity duration-1000 ${isVisible ? "opacity-100" : "opacity-0"
      }`,
      className
    )}
      role="alert"
      {...rest}
    >
      {GetToastIcon(type)}
      <div className="ms-3 mr-4 text-sm font-normal break-words">{message}</div>
      <button
        onClick={onClose}
        type="button"
        className="ms-auto -mx-1.5 -my-1.5 rounded-lg p-1.5 inline-flex items-center justify-center h-8 w-8 text-gray-500 hover:text-white bg-gray-800 hover:bg-gray-700"
        aria-label="Close"
      >
        <Close stroke="2" className="fill-gray-400" />
      </button>
    </div>
  );
};

export default Toast;
