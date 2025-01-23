// src/features/categories/pages/CategoryPanel.tsx
import {
  Box,
  List,
  ListItemButton,
  Collapse,
  Typography,
  CircularProgress,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import { useEffect, useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../hooks";
import { fetchCategories } from "../types/slices/categorySlice";
import { categoryService } from "../types/services/category.services";
import { Category } from "../types/category.types";


const CategoryList = styled(List)(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.shape.borderRadius,
  boxShadow: theme.shadows[1],
}));

const CategoryItem = styled(ListItemButton)(({ theme }) => ({
  "&:hover": {
    backgroundColor: theme.palette.action.hover,
  },
}));

const SubcategoryItem = styled(ListItemButton)(({ theme }) => ({
  paddingLeft: theme.spacing(4),
  "&:hover": {
    backgroundColor: theme.palette.action.hover,
  },
}));

const CategoryText = styled(Typography)(({ theme }) => ({
  color: theme.palette.primary.main,
  fontWeight: 500,
}));

const SubcategoryText = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  fontSize: "0.95rem",
}));

interface CategoryPanelProps {
  onSubcategorySelect: (subcategoryId: number | null) => void;
}

const CategoryPanel: React.FC<CategoryPanelProps> = ({ onSubcategorySelect }) => {
  const dispatch = useAppDispatch();
  const { categories, loading, error } = useAppSelector(
    (state) => state.categories
  );
  const [openCategories, setOpenCategories] = useState<number[]>([]);
  const [loadingSubcategories, setLoadingSubcategories] = useState<number[]>(
    []
  );
  const [categoryDetails, setCategoryDetails] = useState<{
    [key: number]: Category;
  }>({});
 

  const handleCategoryClick = async (categoryId: number) => {
    if (!openCategories.includes(categoryId)) {
      if (!categoryDetails[categoryId]) {
        try {
          setLoadingSubcategories((prev) => [...prev, categoryId]);
          const categoryWithSubs = await categoryService.getWithSubcategories(
            categoryId
          );
          setCategoryDetails((prev) => ({
            ...prev,
            [categoryId]: categoryWithSubs,
          }));
        } catch (error) {
          console.error("Error al cargar las subcategorías:", error);
        } finally {
          setLoadingSubcategories((prev) =>
            prev.filter((id) => id !== categoryId)
          );
        }
      }
    }

    setOpenCategories((prev) =>
      prev.includes(categoryId)
        ? prev.filter((id) => id !== categoryId)
        : [...prev, categoryId]
    );
  };

  useEffect(() => {
    dispatch(fetchCategories());
  }, [dispatch]);

  const handleSubcategoryClick = (subcategoryId: number | null) => {
    console.log('Subcategoría seleccionada:', subcategoryId);
    onSubcategorySelect(subcategoryId);
  };

  if (loading) return <Box>Cargando categorías...</Box>;
  if (error) return <Box>Error al cargar las categorías</Box>;

  return (
    <Box>
      <Typography variant="h6" gutterBottom color="primary">
        Categorías
      </Typography>
      <CategoryItem onClick={() => handleSubcategoryClick(null)}>
        <CategoryText>Todos los productos</CategoryText>
      </CategoryItem>
      <CategoryList>
        {categories.map((category) => (
          <Box key={category.id}>
            <CategoryItem
              onClick={() => handleCategoryClick(category.id)}
              disabled={loadingSubcategories.includes(category.id)}
            >
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  width: "100%",
                }}
              >
                <CategoryText>{category.name}</CategoryText>
                {loadingSubcategories.includes(category.id) ? (
                  <CircularProgress size={20} />
                ) : openCategories.includes(category.id) ? (
                  <ExpandLess />
                ) : (
                  <ExpandMore />
                )}
              </Box>
            </CategoryItem>
            <Collapse in={openCategories.includes(category.id)} timeout="auto">
              <List disablePadding>
                {categoryDetails[category.id]?.subcategories?.map(
                  (subcategory) => (
                    <SubcategoryItem
                      key={subcategory.id}
                      onClick={() => {
                        handleSubcategoryClick(subcategory.id);
                        console.log('click en subcategory :', subcategory.id);
                      }}
                    >
                      <SubcategoryText>{subcategory.name}</SubcategoryText>
                    </SubcategoryItem>
                  )
                )}
              </List>
            </Collapse>
          </Box>
        ))}
      </CategoryList>
    </Box>
  );
};

export default CategoryPanel;
