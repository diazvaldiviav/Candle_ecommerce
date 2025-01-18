import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { fetchCategories } from '../types/slices/categorySlice';
import { 
  List, 
  ListItem, 
  ListItemText, 
  Typography, 
  CircularProgress,
  Box,
  Button
} from '@mui/material';
import { Category } from '../types/category.types';
import { useNavigate } from 'react-router-dom';

const CategoryList = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { categories, loading, error } = useAppSelector(state => state.categories);

  useEffect(() => {
    dispatch(fetchCategories());
  }, [dispatch]);

  // Log cuando el componente recibe nuevos datos
  useEffect(() => {
    console.log('Categorías actualizadas:', categories);
  }, [categories]);

  if (loading) {
    console.log('Cargando categorías...');
    return (
      <Box display="flex" justifyContent="center" p={2}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    console.error('Error al cargar categorías:', error);
    return (
      <Typography color="error" p={2}>
        {error}
      </Typography>
    );
  }

  return (
    <List>
      {categories.map((category: Category) => (
        <ListItem key={category.id}>
          <ListItemText
            primary={category.name}
            secondary={category.description}
          />
          <Button variant="contained" color="primary" onClick={() => navigate(`/categories/with-subcategories/${category.id}`)}>
            Ver Detalles
          </Button>
        </ListItem>
      ))}
    </List>
  );
};

export default CategoryList;