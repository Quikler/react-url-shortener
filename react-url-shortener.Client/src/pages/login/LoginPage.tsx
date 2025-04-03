import LoginForm from "./LoginForm";

const LoginPage = () => {
  return (
    <div className="flex flex-col items-center justify-center py-6 px-4">
      <div className="max-w-md w-full py-16">
        <div className="p-8 rounded-2xl shadow dark:shadow-gray-700 shadow-gray-300">
          <h2 className="text-center text-2xl font-bold">Log in</h2>
          <LoginForm />
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
