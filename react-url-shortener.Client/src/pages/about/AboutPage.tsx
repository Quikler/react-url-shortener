import Button from "@src/components/ui/Buttons/Button";
import { AboutService } from "@src/services/api/AboutService";
import AdminComponent from "@src/components/HOC/AdminComponent";
import { useEffect, useReducer, useRef, useState } from "react";
import { handleErrorWithToast } from "@src/utils/helpers";
import LoadingScreen from "@src/components/ui/LoadingScreen";
import { toast } from "@src/hooks/useToast";

type AboutState = {
  about: string;
  buttonText: "Edit" | "Submit";
  isAboutEditable: boolean;
};

type AboutAction = {
  type: "EDIT" | "SUBMIT" | "SET_ABOUT";
  payload: string;
};

const aboutReducer = (state: AboutState, action: AboutAction): AboutState => {
  switch (action.type) {
    case "SET_ABOUT":
      return { ...state, about: action.payload };

    case "EDIT":
      return { ...state, buttonText: "Edit", isAboutEditable: false };

    case "SUBMIT":
      return { ...state, buttonText: "Submit", isAboutEditable: true };

    default:
      return state;
  }
};

const AboutPage = () => {
  const [loading, setLoading] = useState(true);

  const [about, aboutDispatch] = useReducer(aboutReducer, {
    about: "",
    buttonText: "Edit",
    isAboutEditable: false,
  });

  const textAreaRef = useRef<HTMLTextAreaElement | null>(null);
  useEffect(() => {
    if (textAreaRef.current) {
      textAreaRef.current.style.height = "auto";
      textAreaRef.current.style.height = `${textAreaRef.current.scrollHeight}px`;
    }
  }, [about.about]);

  useEffect(() => {
    const abortController = new AbortController();

    const fetchAbout = async () => {
      try {
        const data = await AboutService.getAbout({ signal: abortController.signal });
        aboutDispatch({ type: "SET_ABOUT", payload: data });
      } catch (e: any) {
        handleErrorWithToast(e);
      } finally {
        setLoading(false);
      }
    };

    fetchAbout();

    return () => abortController.abort();
  }, []);

  const handleEditClick = () => aboutDispatch({ type: "SUBMIT", payload: about.about });

  const handleSubmitClick = async () => {
    try {
      const newAbout = await AboutService.updateAbout(about.about);
      aboutDispatch({ type: "EDIT", payload: newAbout });
      toast.success("Url shortener algorithm updated successfully");
    } catch (e: any) {
      handleErrorWithToast(e);
    }
  };

  const buttonClickHandler = about.buttonText === "Edit" ? handleEditClick : handleSubmitClick;

  if (loading) return <LoadingScreen />;

  return (
    <div className="min-h-full py-24">
      <div className="mx-auto flex gap-4 flex-col items-center justify-center">
        <p className="text-2xl font-medium">Url shortener algorithm:</p>
        <div className="flex flex-col p-8 shadow gap-2 bg-gradient-to-r dark:from-gray-700 dark:to-gray-900 from-gray-200 to-gray-200 rounded-2xl border-black">
          <div className="w-2xl">
            <textarea
              readOnly={!about.isAboutEditable}
              ref={textAreaRef}
              className="w-full resize-none min-h-full border-0 whitespace-pre-wrap outline-0 border-gray-300"
              value={about.about ? about.about : "SORRY... no algorithm presented :("}
              onChange={(e) => aboutDispatch({ type: "SET_ABOUT", payload: e.target.value })}
            />
          </div>
          <AdminComponent>
            <Button onClick={buttonClickHandler} className="self-start">
              {about.buttonText}
            </Button>
          </AdminComponent>
        </div>
      </div>
    </div>
  );
};

export default AboutPage;
