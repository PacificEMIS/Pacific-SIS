import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class KeysGenerateService {

  apiKeyGenerated:Subject<number> = new Subject();

  constructor() { }
}
