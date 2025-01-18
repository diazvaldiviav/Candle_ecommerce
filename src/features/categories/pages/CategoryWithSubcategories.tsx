import { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { fetchCategoryWithSubcategories, clearSelectedCategory } from '../types/slices/categorySlice';
import { 
  Box, 
  Typography, 
  CircularProgress, 
  Card, 
  CardContent,
  List,
  ListItem,
  ListItemText,
  Divider,
  Paper
} from '@mui/material';
//import { SubCategory } from '../types/category.types';


const CategoryWithSubcategories = () => {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const { selectedCategory, loading, error } = useAppSelector(state => state.categories);

  useEffect(() => {
    if (id) {
      dispatch(fetchCategoryWithSubcategories(Number(id)));
    }

    return () => {
      dispatch(clearSelectedCategory());
    };
  }, [dispatch, id]);

  // Agrega un console.log para ver qué datos llegan
  console.log('selectedCategory:', selectedCategory);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={2}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Typography color="error" p={2}>
        {error}
      </Typography>
    );
  }

  if (!selectedCategory) {
    return (
      <Typography p={2}>
        No se encontró la categoría
      </Typography>
    );
  }

   // Verificar si subCategories existe
   const subcategories = selectedCategory.subcategories || [];

  return (
    <Box sx={{ p: 3 }}>
      <Paper elevation={3}>
        <Card>
          <CardContent>
            <Typography variant="h4" gutterBottom color="primary">
              {selectedCategory.name}
            </Typography>
            
            <Typography variant="body1" color="text.secondary" paragraph>
              {selectedCategory.description}
            </Typography>

            <Divider sx={{ my: 2 }} />

            <Typography variant="h6" gutterBottom color="primary">
              Subcategorías: 
            </Typography>

            <List>
              {subcategories.map((subcategory) => (
                <ListItem 
                  key={subcategory.id}
                  sx={{
                    bgcolor: 'background.paper',
                    mb: 1,
                    borderRadius: 1,
                    '&:hover': {
                      bgcolor: 'action.hover',
                    },
                  }}
                >
                  <ListItemText
                    primary={
                      <Typography variant="subtitle1" color="primary">
                        {subcategory.name}
                      </Typography>
                    }
                    secondary={
                      <Typography variant="body2" color="text.secondary">
                        {subcategory.description}
                      </Typography>
                    }
                  />
                </ListItem>
              ))}
            </List>

            {subcategories.length === 0 && (
              <Typography color="text.secondary" align="center">
                No hay subcategorías disponibles
              </Typography>
            )}

          
          </CardContent>
        </Card>
      </Paper>
    </Box>
  );
};

export default CategoryWithSubcategories;