import TabsDictionary from "./components/TabsDictionary";
import Languages from "./components/Languages";
import Swagger from "./components/Swagger";

const AppRoutes = [
  /*{
    index: true,
    element: <Home />
  }, */
  {
    //path: '/dictionary',
    index: true, 
    element: <TabsDictionary />
  },
  {
    path:'/languages',
    element: <Languages/>
  },
  {
    path: '/swagger',
    element: <Swagger/>
  }

];

export default AppRoutes;
