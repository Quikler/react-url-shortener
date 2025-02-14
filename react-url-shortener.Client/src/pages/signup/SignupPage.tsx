import SignupForm from "./SignupForm";

const SignupPage = () => {
  return (
    <div className="bg-gray-50">
      <div className="min-h-screen flex flex-col items-center justify-center py-6 px-4">
        <div className="max-w-md w-full py-16">
          <div className="p-8 rounded-2xl bg-white shadow">
            <h2 className="text-gray-800 text-center text-2xl font-bold">Sign up</h2>
            <SignupForm />
          </div>
        </div>
      </div>
    </div>
  );
};

export default SignupPage;
