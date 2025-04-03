import { twMerge } from "tailwind-merge";
import Close from "../svgr/Close";

type ModalProps = {
  bgClass: string;
  title: string;
  isOpen: boolean;
  onClose: (e: React.MouseEvent<HTMLButtonElement>) => void;
  children: React.ReactNode;
};

const Modal = ({ bgClass, isOpen, onClose, title, children }: ModalProps) => {
  if (!isOpen) return null;

  return (
    <>
      <div className={twMerge("fixed w-full h-full top-0 left-0", bgClass)} />
      <div className="fixed inset-0 flex justify-center items-center">
        <div className="bg-gradient-to-r dark:from-gray-700 dark:to-gray-900 from-gray-200 to-gray-200 p-6 rounded-lg shadow-lg">
          <div className="flex justify-between gap-2 items-center">
            <p className="text-2xl">{title}</p>
            <button onClick={onClose}>
              <Close className="dark:fill-gray-200 fill-gray-800" cursor="pointer" />
            </button>
          </div>
          {children}
        </div>
      </div>
    </>
  );
};

export default Modal;
