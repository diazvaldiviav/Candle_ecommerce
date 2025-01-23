// src/pages/ProductsPage.tsx
import { Box, Typography, Container } from '@mui/material';
import { styled } from '@mui/material/styles';
import CategoryPanel from '../../categories/pages/CategoryPanel';
import FilterToolbar from '../../products/component/FilterToolbar';
import { useProducts } from '../../../hooks/useProducts';
import ProductGrid from '../../products/component/ProductGrid';


const ProductsContainer = styled(Container)(({ theme }) => ({
  padding: theme.spacing(3),
  display: 'flex',
  gap: theme.spacing(3),
  [theme.breakpoints.down('md')]: {
    flexDirection: 'column',
  },
}));

const SidePanel = styled(Box)(({ theme }) => ({
  width: 280,
  flexShrink: 0,
  [theme.breakpoints.down('md')]: {
    width: '100%',
  },
}));

const MainContent = styled(Box)({
  flex: 1,
});

const ProductPage = () => {
  const {
    products,
    loading,
    error,
    currentPage,
    totalPages,
    setCurrentPage,
    setSearchQuery,
    setSortOption,
    totalProducts,
    setSelectedSubcategory,
  } = useProducts();

  

  return (
    <Box>
      <Box sx={{ bgcolor: 'background.paper', py: 4 }}>
        <Container>
          <Typography
            variant="h3"
            component="h1"
            color="primary"
            align="center"
            gutterBottom
          >
            Nuestros Productos
          </Typography>
        </Container>
      </Box>

      <ProductsContainer maxWidth="xl">
        <SidePanel>
        <CategoryPanel onSubcategorySelect={setSelectedSubcategory} />
        </SidePanel>
        
        <MainContent>
      <FilterToolbar 
        onSearch={setSearchQuery}
        onSort={setSortOption}
        totalProducts={totalProducts}
      />
      <ProductGrid 
        products={products}
        loading={loading}
        error={error}
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={setCurrentPage}
      />
    </MainContent>
      </ProductsContainer>
    </Box>
  );
};

export default ProductPage;