import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { COMMA, ENTER } from '@angular/cdk/keycodes';

@Component({
  selector: 'vex-mat-chip',
  templateUrl: './mat-chip.component.html',
  styleUrls: ['./mat-chip.component.scss']
})
export class MatChipComponent implements OnInit {
  filteredFruits: Observable<string[]>;
  chipModelList: string[];
  selectable = true;
  removable = true;
  separatorKeysCodes: number[] = [ENTER, COMMA];
  @ViewChild('fruitInput') fruitInput: ElementRef<HTMLInputElement>;
  @Input() labelName: string;
  @Input() placeHolder;
  @Input() titleKey: string;
  @Input() valueKey: string | number;
  @Input() chipId: number;
  @Input() matChipList: string[];
  @Input() matChipAutocompleteList: string;
  @Input() visibleTo: number;
  @Input() recordFor;
  @Input() isReadOnly: boolean;
  @Output() chipModel: EventEmitter<any> = new EventEmitter();
  constructor() { }

  ngOnInit(): void {
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    if (this.matChipList.findIndex(item => item === event.option.value) === -1) {
      this.matChipList.push(event.option.value);
    }
    this.chipModel.emit(this.chipModelList);
  }

  remove(item) {
    const index = this.matChipList.indexOf(item, 0);
    if (index > -1) {
      this.matChipList.splice(index, 1);
    }
    this.chipModel.emit(this.chipModelList);
  }

}
