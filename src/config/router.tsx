import { createBrowserRouter } from 'react-router-dom';
import { MainLayout } from '../component/layout/MainLayout';
import { lazy } from 'react';


// Lazy loading de páginas
 const Home = lazy(() => import('../features/home/pages/Home'));
 const Products = lazy(() => import('../features/products/pages/Products'));
 //const ProductDetail = lazy(() => import('../features/products/pages/ProductDetails'));
 const CategoryList = lazy(() => import('../features/categories/pages/categoryList'));
 const CategoryWithSubcategories = lazy(() => import('../features/categories/pages/CategoryWithSubcategories'));
 //const Login = lazy(() => import('../features/auth/pages/Login'));
// const Register = lazy(() => import('../features/auth/pages/Register'));
const NotFound = lazy(() => import('../component/common/NotFound'));

export const router = createBrowserRouter([
  {
    path: '/',
    element: <MainLayout />,
    errorElement: <NotFound />,
    children: [
      {
        index: true,
        element: <Home />,
      },
      {
        path: 'products',
        children: [
          {
            index: true,
            element: <Products />,
          }
        ],
      },
        {
            path: 'categories',
            element: <CategoryList />,
        },
        {
            path: '/categories/with-subcategories/:id',
            element: <CategoryWithSubcategories />,
        },
     /* {
        path: 'auth',
        children: [
          {
            path: 'login',
            element: <Login />,
          },
          {
            path: 'register',
            element: <Register />,
          },
        ],
      },*/
    ],
  },
]);