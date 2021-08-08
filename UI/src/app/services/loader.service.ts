import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  // progressRef: NgProgressRef;
  
  public isLoading = new BehaviorSubject(false);
  
  constructor() { }

  // startLoading() {
  //   this.progressRef.start();
  // }

  // completeLoading() {
  //   this.progressRef.complete();
  // }
}
