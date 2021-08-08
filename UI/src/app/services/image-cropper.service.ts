import { Injectable } from '@angular/core';
import { BehaviorSubject,Observable, Subject } from 'rxjs';
import {ImageModel} from '../models/image-cropper.model';

@Injectable({
  providedIn: 'root'
})
export class ImageCropperService {
  imageStat:ImageModel=new ImageModel();

  private cropEventSubject = new Subject<any>();
  private unCropEventSubject = new Subject<any>();
  private message = new BehaviorSubject(this.imageStat);
  sharedMessage = this.message.asObservable();

  private imageStatus = new BehaviorSubject(null);
  shareImageStatus = this.imageStatus.asObservable();
  constructor() { }

  sendCroppedEvent(event) {
    this.cropEventSubject.next(event);
  }
  getCroppedEvent(): Observable<any> {
    return this.cropEventSubject.asObservable();
  }

  sendUncroppedEvent(event) {
    this.unCropEventSubject.next(event);
  }
  getUncroppedEvent(): Observable<any> {
    return this.unCropEventSubject.asObservable();
  }

  enableUpload(message: ImageModel) {
    this.message.next(message)
  }

  cancelImage(imageStatus: string) {
    this.imageStatus.next(imageStatus)
  }
}
