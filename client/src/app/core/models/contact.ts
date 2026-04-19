export interface ContactListItem {
  id: string;
  firstName: string;
  lastName: string;
  categoryName: string;
}

export interface ContactDetail {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  categoryId: string;
  categoryName: string;
  subcategoryId: string | null;
  subcategoryName: string | null;
}

export interface CreateContactRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber: string;
  dateOfBirth: string;
  categoryId: string;
  subcategoryId: string | null;
  subcategoryName: string | null;
}

export interface UpdateContactRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string | null;
  phoneNumber: string;
  dateOfBirth: string;
  categoryId: string;
  subcategoryId: string | null;
  subcategoryName: string | null;
}
