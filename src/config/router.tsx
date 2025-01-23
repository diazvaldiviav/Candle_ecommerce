import { createBrowserRouter } from 'react-router-dom';
import { MainLayout } from '../component/layout/MainLayout';
import { lazy } from 'react';


// Lazy loading de páginas
 const Home = lazy(() => import('../component/layout/pages/home/Home'));
 //const ProductDetail = lazy(() => import('../features/products/pages/ProductDetails'));
 const CategoryList = lazy(() => import('../features/categories/pages/categoryList'));
 const CategoryWithSubcategories = lazy(() => import('../features/categories/pages/CategoryWithSubcategories'));
 //const Login = lazy(() => import('../features/auth/pages/Login'));
// const Register = lazy(() => import('../features/auth/pages/Register'));
const NotFound = lazy(() => import('../component/common/NotFound'));
const ProductPage = lazy(() => import('../features/products/pages/ProductPage'));
const ProductDetails = lazy(() => import('../features/products/pages/ProductDetails'));

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
            element: <ProductPage />,
            },
            {
            path: ':id',
            element: <ProductDetails />,
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