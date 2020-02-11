import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-class-program-data',
  templateUrl: './class-program-data.component.html',
  styleUrls: ['./class-program-data.component.scss']
})
export class ClassProgramDataComponent implements OnInit {
  @Input() aclass: any;
  treeData: any = [];
  expanded = false;

  constructor() { }

  ngOnInit() {
    this.setTreeData();
  }

  setTreeData() {
    const themes = this.aclass.themes;
    for (let i = 0; i < themes.length; i++) {
      const theme = themes[i];
      const node = [{id: theme.id, name: theme.name, checked: false, children: []}];
      this.treeData = [...this.treeData, node];
      const lessons = theme.lessons;
      for (let j = 0; j < lessons.length; j++) {
        const lesson = lessons[j];
        const themeChild = {id: lesson.id, name: lesson.name, checked: false, children: []};
        const contents = lesson.contents;
        node[0].children = [...node[0].children, themeChild];
        for (let k = 0; k < contents.length; k++) {
          const content = contents[k];
          const lessonChild = {id: content.id, name: content.name, checked: false};
          themeChild.children = [...themeChild.children, lessonChild];
        }
      }
    }
    console.log(this.treeData);
  }

  onCheck(e: any) {
    console.log('%c Returned checked object ', 'background: #222; color:  #ff8080');
    console.log(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }
  onCheckedKeys(e: any) {
    console.log('%c Returned array with checked checkboxes ', 'background: #222; color: #bada55');
    console.log(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }
  onNodesChanged(e: any) {
    console.log('%c Returned json with marked checkboxes ', 'background: #222; color: #99ccff');
    console.table(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }

}
