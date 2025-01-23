import { Container } from '@mui/material';
// import { useNavigate } from 'react-router-dom';
import { Header } from './Header';
import { CategoryHome } from './CategoryHome';
import { ProductHome } from './ProductHome';

const Home = () => {
 // const navigate = useNavigate();

  return (
    <Container maxWidth="lg">

      {/* <Header /> */}
      <Header />

    {/* seccion de categorias */}

    <CategoryHome />


    {/* seccion de productos destacados*/}
      <ProductHome />

    
    </Container>
  );
};

export default Home;