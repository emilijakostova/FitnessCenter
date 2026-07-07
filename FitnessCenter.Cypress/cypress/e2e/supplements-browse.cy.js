describe('FitnessCenter - Supplements Browse (web scraping)', () => {

  it('Browse страницата ги прикажува сите категории', () => {
    cy.visit('/Supplements/Browse');

    cy.get('.category-link').should('have.length', 5);
    cy.contains('.category-link', 'Протеини').should('exist');
    cy.contains('.category-link', 'Колаген').should('exist');
    cy.contains('.category-link', 'Витамини').should('exist');
  });

  it('клик на категорија ги прикажува scrape-натите производи', () => {
    cy.visit('/Supplements/Browse');

    cy.contains('.category-link', 'Протеини').click();

    cy.url({ timeout: 20000 }).should('include', '/Supplements/BrowseCategory');
    cy.contains('Суплементи -', { timeout: 20000 }).should('exist');

    cy.get('table tbody tr', { timeout: 20000 }).should('have.length.greaterThan', 0);

    cy.get('table tbody tr').first().within(() => {
      cy.get('img').should('have.attr', 'src');
      cy.get('td').eq(1).invoke('text').should('not.be.empty');
      cy.contains('Погледни').should('have.attr', 'href');
    });
  });

  it('прикажува "На залиха" или "Нема" за секој производ', () => {
    cy.visit('/Supplements/Browse');
    cy.contains('.category-link', 'Протеини').click();

    cy.get('table tbody tr', { timeout: 20000 }).each(($row) => {
      cy.wrap($row).should('contain.text', 'залиха').should('satisfy', ($el) => {
        const text = $el.text();
        return text.includes('На залиха') || text.includes('Нема');
      });
    });
  });

  it('пагинацијата работи кога има повеќе од една страна', () => {
    cy.visit('/Supplements/Browse');
    cy.contains('.category-link', 'Протеини').click();

    cy.contains('Страна', { timeout: 20000 }).invoke('text').then((text) => {
      const match = text.match(/Страна (\d+) од (\d+)/);
      const totalPages = match ? parseInt(match[2]) : 1;

      if (totalPages > 1) {
        cy.contains('Следна').click();
        cy.url({ timeout: 20000 }).should('include', 'page=2');
      } else {
        cy.log('Само 1 страна достапна за оваа категорија — пагинацијата не може да се тестира со реалниот сет производи.');
      }
    });
  });

  it('непостоечка категорија враќа 404', () => {
    cy.request({
      url: '/Supplements/BrowseCategory?categoryName=НепостоечкаКатегорија&page=1',
      failOnStatusCode: false
    }).its('status').should('eq', 404);
  });

});