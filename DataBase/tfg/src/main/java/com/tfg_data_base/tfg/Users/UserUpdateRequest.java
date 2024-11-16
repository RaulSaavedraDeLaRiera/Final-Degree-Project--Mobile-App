package com.tfg_data_base.tfg.Users;

public class UserUpdateRequest {
        private String fieldName;  
        private Object newValue; 
    
        public String getFieldName() {
            return fieldName;
        }
    
        public void setFieldName(String fieldName) {
            this.fieldName = fieldName;
        }
    
        public Object getNewValue() {
            return newValue;
        }
    
        public void setNewValue(Object newValue) {
            this.newValue = newValue;
        }
}