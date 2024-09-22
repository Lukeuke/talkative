import { Home } from "./components/Home";
import SignIn from "./pages/identity/SignIn";
import SignUp from "./pages/identity/SignUp";
import GroupPage from "./pages/group/Group";
import Test from "./pages/test/Test";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: "/sign-in",
    element: <SignIn />
  },
  {
    path: "/sign-up",
    element: <SignUp />
  },
  {
    path: "/group/:id",
    element: <GroupPage />
  },
  {
    path: "/test",
    element: <Test />
  }
];

export default AppRoutes;
