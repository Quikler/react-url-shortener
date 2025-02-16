import Close from "../svgr/Close";

type ModalProps = {
  title: string;
  isOpen: boolean;
  onClose: (e: React.MouseEvent<HTMLButtonElement>) => void;
  children: React.ReactNode;
};

const Modal = ({ isOpen, onClose, title, children }: ModalProps) => {
  if (!isOpen) return null;

  return (
    <>
      <div className="fixed bg-white opacity-75 w-full h-full top-0 left-0" />
      <div className="fixed inset-0 flex justify-center items-center">
        <div className="bg-white p-6 rounded-lg shadow-lg">
          <div className="flex justify-between gap-2 items-center">
            <p className="text-2xl">{title}</p>
            <button onClick={onClose}>
              <Close cursor="pointer" />
            </button>
          </div>
          {children}
        </div>
      </div>
    </>
  );
};

export default Modal;
