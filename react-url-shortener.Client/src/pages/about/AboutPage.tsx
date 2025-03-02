import Button from "@src/components/ui/Button";
import { AboutService } from "@src/services/api/AboutService";
import AdminComponent from "@src/services/routes/AdminComponent";
import { useEffect, useReducer, useRef } from "react";

type AboutState = {
  about: string;
  buttonText: "Edit" | "Submit";
  //buttonClick: () => void;
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
  const [about, aboutDispatch] = useReducer(aboutReducer, {
    about: "",
    buttonText: "Edit",
    isAboutEditable: false,
  });

  const textAreaRef = useRef<HTMLTextAreaElement | null>(null);
  const codeRef = useRef<HTMLPreElement | null>(null);

  useEffect(() => {
    if (textAreaRef.current) {
      textAreaRef.current.style.height = "auto";
      textAreaRef.current.style.height = `${textAreaRef.current.scrollHeight}px`;
    }
  }, [about.about, about.isAboutEditable]);

  useEffect(() => {
    const abortController = new AbortController();

    const fetchAbout = async () => {
      try {
        const data = await AboutService.getAbout({ signal: abortController.signal });
        aboutDispatch({ type: "SET_ABOUT", payload: data });
      } catch (e: any) {
        console.error("Cannot get about:", e.message);
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
    } catch (e: any) {
      console.error("Cannot update about:", e.message);
    }
  };

  const buttonClickHandler = about.buttonText === "Edit" ? handleEditClick : handleSubmitClick;

  return (
    <div className="min-h-screen py-24">
      <div className=" mx-auto flex gap-4 flex-col items-center justify-center">
        <p className="text-2xl font-medium">About algorithm:</p>
        <div className="flex flex-col gap-2">
          <div className="w-2xl p-4 border border-black">
            {about.isAboutEditable ? (
              <textarea
                ref={textAreaRef}
                className="w-full min-h-full p-2 border-0 outline-0 border-gray-300"
                value={about.about}
                onChange={(e) => aboutDispatch({ type: "SET_ABOUT", payload: e.target.value })}
              />
            ) : (
              <code ref={codeRef} className="whitespace-pre-wrap">
                {about.about}
              </code>
            )}
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
