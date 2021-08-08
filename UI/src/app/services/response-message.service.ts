import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ResponseMessageService {

  constructor(public snackBar: MatSnackBar) { }
   
  showSuccess(message: string): void {
    this.snackBar.open(message);
  }
  
  showError(message: string): void {
    // The second parameter is the text in the button. 
    // In the third, we send the css class, duration for the snack bar.
    this.snackBar.open(message, 'X', { duration: 8000});
  }
}
