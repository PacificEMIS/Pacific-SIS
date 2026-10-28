import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ConfigService } from '../../services/config.service';
import { map } from 'rxjs/operators';
import icHelp from '@iconify/icons-ic/help';
import icSearch from '@iconify/icons-ic/search';
import icDropdown from '@iconify/icons-ic/arrow-drop-down';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-secondary-toolbar',
  templateUrl: './secondary-toolbar.component.html',
  styleUrls: ['./secondary-toolbar.component.scss']
})
export class SecondaryToolbarComponent implements OnInit {

  icHelp = icHelp;
  icSearch = icSearch;
  icDropdown = icDropdown;
  @Input() pages:string[] = [];
  @Input() schoolSettings: boolean = false;
  @Output() selectedPage = new EventEmitter<string>();
  @Input() current: string;
  @Input() crumbs: string[];
  selectedValue:string;

  fixed$ = this.configService.config$.pipe(
    map(config => config.toolbar.fixed)
  );

  constructor(private configService: ConfigService, private defaultValuesService: DefaultValuesService,
    public translateService:TranslateService,
    ) {
   }
  
  ngOnInit() {
    this.selectedValue=this.defaultValuesService.getPageId();
  }
  
  changePage(pageName){
    this.selectedValue = pageName; 
    this.defaultValuesService.setPageId(pageName);
    this.selectedPage.emit(pageName)
  }
}
