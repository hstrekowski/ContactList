export interface Category {
  id: string;
  name: string;
}

export interface Subcategory {
  id: string;
  name: string;
  categoryId: string;
}

export const CATEGORY_NAMES = {
  business: 'Służbowy',
  private: 'Prywatny',
  other: 'Inny',
} as const;
