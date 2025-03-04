import { twMerge } from "tailwind-merge";
import CheckIcon from "../svgr/CheckIcon";
import Close from "../svgr/Close";
import ErrorIcon from "../svgr/ErrorIcon";
import WarningIcon from "../svgr/WarningIcon";

export type ToastType = "success" | "danger" | "warning";

type ToastProps = React.HTMLAttributes<HTMLDivElement> & {
  message?: string | null;
  type: ToastType;
  isVisible: boolean;
  setIsVisible: React.Dispatch<React.SetStateAction<boolean>>;
  onClose: () => void;
};

const getToastIcon = (type: ToastType): JSX.Element => {
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
  setIsVisible,
  onClose,
  ...rest
}: ToastProps) => {
  const handleClose = () => {
    setIsVisible(false);
    onClose();
  };

  return (
    <div
      className={twMerge(
        `fixed top-24 right-12 z-[999] flex items-center w-full max-w-xs p-4 rounded-lg shadow-sm text-gray-400 bg-gray-800 transition-opacity duration-1000 ${
          isVisible ? "opacity-100" : "opacity-0"
        }`,
        className
      )}
      role="alert"
      {...rest}
    >
      {getToastIcon(type)}
      <div className="ms-3 text-sm font-normal break-words">{message}</div>
      <button
        onClick={handleClose}
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